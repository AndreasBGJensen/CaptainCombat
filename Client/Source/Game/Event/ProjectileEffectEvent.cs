﻿using CaptainCombat.Client;
using CaptainCombat.Common;

namespace CaptainCombat.Client {

    public class ProjectileEffectEvent : Event {

        public ProjectileEffectEvent() { }

        public GlobalId TargetId { get; set; }
        public uint Damage { get; set; }

        public ProjectileEffectEvent(uint receiver, GlobalId targetId, uint damage) : base(receiver) {
            TargetId = targetId;
            Damage = damage;
        }
    }

}