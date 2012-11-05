using System;
using ResEx.Core;
using ResEx.Core.PlugIns;
using ResEx.StandardPlugIns.Properties;

namespace ResEx.StandardPlugIns
{
    [PlugIn(Name = "Plug-Ins Overview",
            Author = "Dimitris Papadimitriou",
            Description = "Displays a list of available plugins")]
    public class PlugInsOverviewPlugIn : IPlugIn
    {
        private IContext context;

        private ButtonInfo button;

        public void Initialize(IContext context)
        {
            this.context = context;

            // create the button that will invoke the plug in
            this.button = new ButtonInfo();
            this.button.Caption = "Plug-Ins Overview";
            this.button.Image = Resources.PlugInImage;
            this.button.OnClick += this.Invoke;
            this.button.ToolBarVisible = false;
            this.button.ToolTip = "Displays a list of loaded plug ins";
            this.context.AddButton(this.button);
        }

        // invokes the plug in when button is pressed
        private void Invoke(object sender, EventArgs e)
        {
            using (var form = new PlugInsOverviewPlugInForm(this.context.PlugIns))
            {
                form.ShowDialog();
            }
        }
    }
}