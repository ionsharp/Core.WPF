using System;
using System.Management;

namespace Imagin.Core.Storage;

public sealed class RemovableDriveEventArgs : EventArgs
{
    public readonly string Name;

    public RemovableDriveEventArgs(string path) => Name = path;
}

public delegate void RemovableDriveEventHandler(RemovableDriveEventArgs e);

public class RemovableDrive
{
    public static event RemovableDriveEventHandler Inserted;

    public static event RemovableDriveEventHandler Removed;

    public enum EventType
    {
        Inserted = 2,
        Removed = 3
    }

    static RemovableDrive()
    {
        ManagementEventWatcher watcher = new ManagementEventWatcher();
        WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 or EventType = 3");

        watcher.EventArrived += (s, e) =>
        {
            string driveName = e.NewEvent.Properties["DriveName"].Value.ToString();
            EventType eventType = (EventType)(Convert.ToInt16(e.NewEvent.Properties["EventType"].Value));

            string eventName = Enum.GetName(typeof(EventType), eventType);
            
            if (eventType == EventType.Inserted)
                Inserted?.Invoke(new(driveName));

            if (eventType == EventType.Removed)
                Removed?.Invoke(new(driveName));
        };

        watcher.Query = query;
        watcher.Start();
    }
}