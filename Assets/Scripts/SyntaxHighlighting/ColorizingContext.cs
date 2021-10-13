using System;
using System.Text;

namespace CodeAnimator
{
    class ColorizingContext
    {
        public readonly StringBuilder StringBuilder;
        public readonly Style Style;
        private bool isColorizeIdentifierName = false;
        private bool isColorizeMemberInvocation = false;

        public bool IsColorizeIdentifierName => isColorizeIdentifierName;
        public bool IsColorizeMemberInvocation => isColorizeMemberInvocation;
        
        public ColorizeIdentifierNameScope ColorizeIdentifierName => new ColorizeIdentifierNameScope(this);
        public ColorizeMemberInvocationScope ColorizeMemberInvocation => new ColorizeMemberInvocationScope(this);

        public void ResetColorizeMemberInvocation()
        {
            isColorizeMemberInvocation = false;
        }

        public ColorizingContext(Style style)
        {
            StringBuilder = new StringBuilder();
            Style = style;
        }

        public struct ColorizeIdentifierNameScope : IDisposable
        {
            private readonly ColorizingContext context;
            private readonly bool enterValue;

            public ColorizeIdentifierNameScope(ColorizingContext context)
            {
                this.context = context;
                enterValue = context.isColorizeIdentifierName;
                context.isColorizeIdentifierName = true;
            }

            public void Dispose()
            {
                context.isColorizeIdentifierName = enterValue;
            }
        }
        
        public struct ColorizeMemberInvocationScope : IDisposable
        {
            private readonly ColorizingContext context;
            private readonly bool enterValue;

            public ColorizeMemberInvocationScope(ColorizingContext context)
            {
                this.context = context;
                enterValue = context.isColorizeMemberInvocation;
                context.isColorizeMemberInvocation = true;
            }

            public void Dispose()
            {
                context.isColorizeMemberInvocation = enterValue;
            }
        }
    }
}