using Assets.Common;
using Assets.Entity;
using Assets.Entity.Controllers;
using Assets.Entity.Hull;
using Assets.Entity.Interfaces;
using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Actions;
using Entity.Controllers.AI;
using Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Controllers
{
    public class EntityController : MonoBehaviour, IObject, IAbbility
    {
        public EntityDataContainer data;
        public TotalAbbilitiesController abbilitiesController;
        public EntityAssembler Assembler { get; private set; }
        public TotalAbbilitiesController totalAbbilitiesController { get; private set; }
        public StatModController statModController { get; private set; } = new();
        public BuffStatusesController Buffs { get; private set; }
        public IDriver Driver { get; set; }
        [SerializeField] private GameObject _despawnPrefab;
        public string Id { get; set; }
        public HullBase hull;

        public EntitySnapshot GetSnapshot() => new EntitySnapshot(this, data);

        private void Update()
        {
            if (hull == null) return;
            Driver?.UpdateControl(this);
        }

        #region Setup

        private void Awake()
        {
            Assembler = new EntityAssembler(this);
            abilitiesController = new(totalAbbilitiesController);

            Id = GameObjectHandler.GenerateUniqueId(name);
            if (GameObjectHandler.GetAI(this) == null) GameObjectHandler.RegisterPlayer(this);
        }

        public void Setup(EntityDataContainer data)
        {
            if (data == null) return;

            this.data.equipmentIds = data.equipmentIds;
            Assembler.SetHull(data.hullId);

            var dPosition = data.position;
            if (dPosition != Vector2.zero)
                transform.position = dPosition;
        }

        public void SetupScripts(params ScriptBase[] scripts)
        {
            Driver = gameObject.AddComponent<AiController>();
            if (Driver is AiController aiController)
            {
                aiController.Scripts = new Queue<ScriptBase>(scripts);
            }
        }

        #endregion

        #region IAbbility
        public AbilitiesController abilitiesController;
        public IReadOnlyList<AbilityUnit> RuntimeAbilities => abilitiesController.RuntimeAbilities;

        public void AddAbility(AbilityUnit ability) => abilitiesController.AddAbility(ability);

        public bool RemoveAbility(AbilityUnit ability) => abilitiesController.RemoveAbility(ability);

        public void Activate(Vector2 targetPos, AbilityUnit abilityUnit, InteractionContext context)
        {
            if (abilitiesController.TryActivate(targetPos, abilityUnit, context))
                EventBrocker.Raise(new EntityInteractionEvent(context));
        }

        #endregion

        #region Buffs
            

        #endregion
    }
}
