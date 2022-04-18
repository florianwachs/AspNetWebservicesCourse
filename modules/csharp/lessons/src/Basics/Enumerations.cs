using Xunit;

namespace Basics;

public class Enumerations
{
    [Fact]
    public void Demo1()
    {
        Console.WriteLine(DocumentOptions.MarkAsConfidential | DocumentOptions.Archive | DocumentOptions.ConvertToPDF);
    }
}

#region Beispiel 1
public enum TaskStates
{
    New,        // 0
    Committed,  // 1
    InProgress, // 2
    Done,       // 3
}

public class Task
{
    public TaskStates State { get; set; }

    public Task()
    {
        State = TaskStates.New;
    }

    public void SetNewState(TaskStates newState)
    {
        switch (newState)
        {
            case TaskStates.New:
                break;
            case TaskStates.Committed:
                break;
            case TaskStates.InProgress:
                break;
            case TaskStates.Done:
                break;
            default:
                break;
        }
    }
}

#endregion

#region Beispiel 2

// das Flags-Attribut hat nur Auswirkung in der ToString()-Methode
[Flags]
public enum DocumentOptions
{
    ConvertToPDF = 1,
    SendAsMail = 2,
    Archive = 4,
    MarkAsConfidential = 8,
    Default = ConvertToPDF | Archive,
}

public class DocumentManager
{
    public void Foo(string documentText, DocumentOptions options)
    {
        if ((options & DocumentOptions.ConvertToPDF) != 0)
        {
            // foo
        }

        if (options.HasFlag(DocumentOptions.MarkAsConfidential))
        {
            // send to NSA :-)
        }
    }
}

#endregion

