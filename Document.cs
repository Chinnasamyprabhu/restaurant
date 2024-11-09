using System;

namespace restaurant.User
{
    internal class Document
    {
        private object a4;

        public Document(object a4)
        {
            this.a4 = a4;
        }

        public Document()
        {
        }

        internal void Add(Paragraph paragraph)
        {
            throw new NotImplementedException();
        }

        internal void Open()
        {
            throw new NotImplementedException();
        }

        internal void Add(PdfPTable table)
        {
            throw new NotImplementedException();
        }

        internal void Close()
        {
            throw new NotImplementedException();
        }
    }
}