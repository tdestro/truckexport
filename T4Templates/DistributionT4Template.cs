﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 12.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace truck_manifest.T4Templates
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "12.0.0.0")]
    public partial class DistributionT4Template : DistributionT4TemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("\r\n");
            
            #line 8 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
 if(ProductsDrawGrandTotalsHash.Count > 0){

 
            
            #line default
            #line hidden
            this.Write("\r\n\r\n");
            
            #line 13 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
    if(TruckDictionary["UseHeader"] == "True"){

 
            
            #line default
            #line hidden
            this.Write(" <tr>\r\n <td colspan=\"13\" align=\"center\"><b>Alternate Product Distribution for ");
            
            #line 17 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(TruckDictionary["RunDate"]));
            
            #line default
            #line hidden
            this.Write(@"</b>
</td>
 </tr>
 <tr class=""title"">
 <td>Key</td>
 <td>Truck</td>
 <td>Delivery Area</td>
  <td>WSJ</td>
  <td>FTU</td>
  <td>IBD</td>
  <td>NYT</td>
  <td>STL</td>
  <td>TWP</td>
  <td>USA</td>
  <td>USW</td>
  <td>USE</td>
  <td>BAR</td>
 </tr>
 ");
            
            #line 35 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
 } 
            
            #line default
            #line hidden
            this.Write("  <tr>\r\n  <td>");
            
            #line 37 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(TruckDictionary["ShortTruckID"]));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  <td>");
            
            #line 38 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(TruckDictionary["TruckName"]));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  <td>");
            
            #line 39 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(TruckDictionary["DeliveryArea"].Split(new string[]{"<BR>"}, StringSplitOptions.None)[0]));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  <td>");
            
            #line 40 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tally(ProductsDrawGrandTotalsHash,"WSJ")));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  <td>");
            
            #line 41 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tally(ProductsDrawGrandTotalsHash,"FTU")));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  <td>");
            
            #line 42 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tally(ProductsDrawGrandTotalsHash,"IBD")));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  <td>");
            
            #line 43 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tally(ProductsDrawGrandTotalsHash,"NYT")));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  <td>");
            
            #line 44 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tally(ProductsDrawGrandTotalsHash,"STL")));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  <td>");
            
            #line 45 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tally(ProductsDrawGrandTotalsHash,"TWP")));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  <td>");
            
            #line 46 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tally(ProductsDrawGrandTotalsHash,"USA")));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  <td>");
            
            #line 47 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tally(ProductsDrawGrandTotalsHash,"USW")));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  <td>");
            
            #line 48 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tally(ProductsDrawGrandTotalsHash,"USE")));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  <td>");
            
            #line 49 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(tally(ProductsDrawGrandTotalsHash,"BAR")));
            
            #line default
            #line hidden
            this.Write("</td>\r\n  </tr>\r\n   ");
            
            #line 51 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
 } 
            
            #line default
            #line hidden
            this.Write("     ");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 52 "C:\Users\IEUser\Desktop\truckexport\T4Templates\DistributionT4Template.tt"
 
	 
   private int tally(Dictionary<string, Dictionary<string, Dictionary<string, int>>> ProductsDrawGrandTotalsHash, string prod)
        {
            if (!ProductsDrawGrandTotalsHash.ContainsKey(prod)) return 0;
            int total = 0;
            foreach (KeyValuePair<string, Dictionary<string, int>> papersection in ProductsDrawGrandTotalsHash[prod])
            {
                total += papersection.Value["Draw"];
            }
            return total;
        }

public StringDictionary TruckDictionary {get; set; }
    public Dictionary<string, Dictionary<string,Dictionary<string, int>>> ProductsDrawGrandTotalsHash {get; set;}
    public bool altprod{get; set;}

        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "12.0.0.0")]
    public class DistributionT4TemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
