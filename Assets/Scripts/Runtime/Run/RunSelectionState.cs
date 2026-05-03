using Animalis.Characters;

namespace Animalis.Run
{
    public static class RunSelectionState
    {
        public static CharacterDefinition SelectedCharacter { get; private set; }

        public static void SetCharacter(CharacterDefinition character)
        {
            SelectedCharacter = character;
        }

        public static void Clear()
        {
            SelectedCharacter = null;
        }
    }
}
