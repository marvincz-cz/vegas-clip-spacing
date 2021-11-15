using System.Collections.Generic;
using System.Windows.Forms;
using ScriptPortal.Vegas;

public class EntryPoint
{
    /// SPACE BETWEEN CLIPS IN SECONDS
    Timecode offset = Timecode.FromSeconds(8.0);
    public void FromVegas(Vegas vegas)
    {
        List<TrackEvent> selected = FindSelectedEvents(vegas.Project);

        if (selected.Count > 0)
        {
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

            MessageBox.Show("Added " + (offset.ToMilliseconds() / 1000) + " second spacing to " + selected.Count + " events.");
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