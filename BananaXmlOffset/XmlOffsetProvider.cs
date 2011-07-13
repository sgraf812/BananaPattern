using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using BananaPattern;
using BananaXmlOffset.Common;
using BananaXmlOffset.XmlElements;

namespace BananaXmlOffset
{
    public class XmlOffsetProvider : CachedNamedOffsetProvider
    {
        private static readonly string _patternFile = Helper.GetAssemblyRootedPath("Patterns.xml");
        private static readonly string _schemaFile = Helper.GetAssemblyRootedPath("Patterns.xsd");
        internal XDocument _patternDocument;
        private readonly IFileInfoAdapter _fileInfo;
        private bool _needsDocumentUpdate;
        private DateTime _lastDocumentWriteTime;
        private readonly IBotProcessContext _context;
        private IMemory Memory { get { return _context.Memory; } }

        public XmlOffsetProvider(IBotProcessContext context)
            : this(context, new FileInfoAdapter(_patternFile))
        {
        }

        internal XmlOffsetProvider(IBotProcessContext context, IFileInfoAdapter fileInfo)
        {
            _context = context;
            _fileInfo = fileInfo;
            ReloadDocument();
            _patternDocument.Changed += PatternDocument_Changed;
        }

        private void PatternDocument_Changed(object sender, XObjectChangeEventArgs e)
        {
            _needsDocumentUpdate = true;
        }

        private void ReloadDocument()
        {
            Stream documentStream = _fileInfo.OpenRead();
            LoadOrBuildValidDocument(documentStream);

            _lastDocumentWriteTime = _fileInfo.LastWriteTime;
        }

        private void LoadOrBuildValidDocument(Stream documentStream)
        {
            if (documentStream.Length > 0)
            {
                _patternDocument = XDocument.Load(documentStream);
                ValidatePatternDocument();
            }
            else
            {
                BuildValidPatternDocument();
            }
        }

        private void BuildValidPatternDocument()
        {
            _patternDocument = new XDocument(
                new XDeclaration("1.0", "utf-8", "no"),
                CreateRootElement()
            );
        }

        private static XElement CreateRootElement()
        {
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            return new XElement("Patterns",
                new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XAttribute(xsi + "noNamespaceSchemaLocation", "Patterns.xsd"));
        }

        private void ValidatePatternDocument()
        {
            var schemas = new XmlSchemaSet();
            schemas.Add(null, _schemaFile);

            _patternDocument.Validate(schemas,
                (s, e) => { throw new OffsetException("Invalid XML offset document schema", e.Exception); }
            );
        }

        public override bool CanResolve(string name)
        {
            bool isCached = base.CanResolve(name);

            OffsetElement offsetElement = GetOffsetElementForName(name);

            return isCached || offsetElement != null;
        }

        private OffsetElement GetOffsetElementForName(string name)
        {
            return _patternDocument.Root.Elements("Offset")
                       .Select(ele => new OffsetElement(ele))
                       .FirstOrDefault(offs => offs.Name == name);
        }

        protected override IntPtr CalculateAddress(string name, out bool addResultToCache)
        {
            if (WasPatternFileModified) ReloadDocument();

            OffsetElement offsetElement = GetOffsetElementOrThrow(name);
            IntPtr address = CalculateOffset(offsetElement);

            if (_needsDocumentUpdate) SaveAndReloadDocument();

            addResultToCache = true;
            return address;
        }

        private bool WasPatternFileModified
        {
            get
            {
                return _fileInfo.LastWriteTime > _lastDocumentWriteTime;
            }
        }

        private OffsetElement GetOffsetElementOrThrow(string name)
        {
            OffsetElement offsetElement = GetOffsetElementForName(name);
            if (offsetElement == null)
                throw new OffsetException("Offset for " + name + " could not be resolved. There was no Offset element.");

            return offsetElement;
        }

        private IntPtr CalculateOffset(OffsetElement offsetElement)
        {
            string result = offsetElement.RootOperation.Execute(_context);
            IntPtr address = new IntPtr(Convert.ToInt64(result, 0x10));
            return address;
        }

        private void SaveAndReloadDocument()
        {
            Stream documentStream = _fileInfo.OpenWrite();
            documentStream.SetLength(0);
            _patternDocument.Save(documentStream);
            _needsDocumentUpdate = false;
            ReloadDocument();
        }
    }
}
