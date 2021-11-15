using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ScriptPortal.Vegas;

namespace vegas_clip_spacing
{
    public partial class Form1 : Form
    {
        public double OffsetSeconds;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Submit();
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Return)
            {
                Submit();
            }
        }

        private void Submit()
        {
            OffsetSeconds = (double) numericUpDown1.Value;
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    public class EntryPoint
    {
        public void FromVegas(Vegas vegas)
        {
            List<TrackEvent> selected = FindSelectedEvents(vegas.Project);

            if (selected.Count > 0)
            {
                double offsetSeconds;
                using (Form1 form1 = new Form1())
                {
                    DialogResult result = form1.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        offsetSeconds = form1.OffsetSeconds;
                    }
                    else
                    {
                        return;
                    }
                }

                var offset = Timecode.FromSeconds(offsetSeconds);
                
                // sort by Start, just in case
                selected.Sort((first, second) => first.Start.CompareTo(second.Start));

                // first clip is also moved
                Timecode lastEnd = selected[0].Start;
                foreach (TrackEvent trackEvent in selected)
                {
                    // move clip start to the end of the previous one plus offset
                    trackEvent.AdjustStartLength(lastEnd + offset, trackEvent.Length, true);
                    lastEnd = trackEvent.End;
                }
            }
            else
            {
                MessageBox.Show("No events selected.");
            }
        }
        
        /// get all selected events
        List<TrackEvent> FindSelectedEvents(Project project)
        {
            List<TrackEvent> selectedEvents = new List<TrackEvent>();
            foreach (Track track in project.Tracks)
            {
                foreach (TrackEvent trackEvent in track.Events)
                {
                    if (trackEvent.Selected)
                    {
                        selectedEvents.Add(trackEvent);
                    }
                }
            }
            return selectedEvents;
        }
    }
}