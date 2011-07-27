using System;

namespace NodaTime.Format.Builder
{
    internal abstract class AbstractFormatterBuilderNode<TInfo> : IFormatterBuilderNode<TInfo> where TInfo : ParseInfo
    {
        private readonly IFormatNode<TInfo> formatNode;

        protected AbstractFormatterBuilderNode(IFormatNode<TInfo> formatNode)
        {
            this.formatNode = formatNode;
        }

        #region IFormatterBuilderNode<TInfo> Members
        public virtual IFormatNode<TInfo> MakeNode(IFormatProvider formatProvider)
        {
            return formatNode;
        }
        #endregion
    }
}