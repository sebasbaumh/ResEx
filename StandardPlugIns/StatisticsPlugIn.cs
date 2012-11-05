using System;
using ResEx.Core;
using ResEx.Core.PlugIns;

namespace ResEx.StandardPlugIns
{
    [PlugIn(Name = "Statistics",
            Author = "Dimitris Papadimitriou",
            Description = "Displays statistics about the current project",
            Disabled = true)]
    public class StatisticsPlugIn : IPlugIn
    {
        private IContext context;

        private ButtonInfo button;

        public void Initialize(IContext context)
        {
            this.context = context;

            // create the button that will invoke the plug in
            this.button = new ButtonInfo();
            this.button.Caption = "Statistics";
            this.button.Image = null;
            this.button.OnClick += this.Invoke;
            this.button.ToolBarVisible = false;
            this.button.ToolTip = "Displays statistics about the current project";
            this.button.Context = PluginContext.ResourceBundle.ToString(); // plug in can be invoke if there is something loaded
            this.context.AddButton(this.button);
        }

        private void Invoke(object sender, EventArgs e)
        {
            using (var form = new StatisticsPlugInForm(this.context))
            {
                form.ShowDialog();
            }
        }
    }
}