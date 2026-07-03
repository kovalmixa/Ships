using Actions;
using Assets.Entity.Modifiers;

namespace Assets.UI.SkillTree
{
    public class SkillNode
    {
        public string skillName;
        public TemplateActionBase[] actions;
        public BuffStatus[] statuses;

        public uint level;
        public uint maxLevel;
        public uint reqPlayerLevel;
        public uint cost;

        public uint minPrevQuantity = 1;
    }
}
