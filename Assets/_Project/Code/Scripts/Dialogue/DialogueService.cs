namespace Novel.Dialogue
{
    public interface IDialogueService
    {
        DialogueSO Dialogue { get; }
        void SetDialogueData(DialogueSO dialogue);
    }

    public sealed class DialogueService : IDialogueService
    {
        public DialogueSO Dialogue { get; private set; }

        public void SetDialogueData(DialogueSO dialogue)
        {
            Dialogue = dialogue;
        }
    }
}