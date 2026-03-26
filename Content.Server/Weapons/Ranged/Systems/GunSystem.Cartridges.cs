using Content.Server.Popups;
using Content.Shared.Damage;
using Content.Shared.Damage.Events;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Prototypes;

namespace Content.Server.Weapons.Ranged.Systems;

public sealed partial class GunSystem
{
    protected override void InitializeCartridge()
    {
        base.InitializeCartridge();
        SubscribeLocalEvent<CartridgeAmmoComponent, ExaminedEvent>(OnCartridgeExamine);
        SubscribeLocalEvent<CartridgeAmmoComponent, DamageExamineEvent>(OnCartridgeDamageExamine);
    }

    private void OnCartridgeDamageExamine(EntityUid uid, CartridgeAmmoComponent component, ref DamageExamineEvent args)
    {
        var damageSpec = GetProjectileDamage(component.Prototype);

        if (damageSpec == null)
            return;

        _damageExamine.AddDamageExamine(args.Message, Damageable.ApplyUniversalAllModifiers(damageSpec), Loc.GetString("damage-projectile"));
    }

    private DamageSpecifier? GetProjectileDamage(string proto)
    {
        if (!ProtoManager.TryIndex<EntityPrototype>(proto, out var entityProto))
            return null;

        if (entityProto.Components
            .TryGetValue(Factory.GetComponentName<ProjectileComponent>(), out var projectile))
        {
            var p = (ProjectileComponent) projectile.Component;

            if (!p.Damage.Empty)
            {
                return p.Damage * Damageable.UniversalProjectileDamageModifier;
            }
        }

        return null;
    }

    // Mono start
    private bool GetProjectileIgnoreResistances(string proto)
    {
        if (!ProtoManager.TryIndex<EntityPrototype>(proto, out var entityProto))
            return false;

        if (entityProto.Components
            .TryGetValue(Factory.GetComponentName<ProjectileComponent>(), out var projectile))
        {
            var p2 = (ProjectileComponent) projectile.Component;

            if (!p2.IgnoreResistances != true)
            {
                return true;
            }
        }

        return false;
    }

    private float GetProjectileArmorPenetration(string proto) // Mono
    {
        if (!ProtoManager.TryIndex<EntityPrototype>(proto, out var entityProto))
            return 0;

        if (entityProto.Components
            .TryGetValue(Factory.GetComponentName<ProjectileComponent>(), out var projectile))
        {
            var p3 = (ProjectileComponent) projectile.Component;

            if (p3.ArmorPenetration != 0)
            {
                return p3.ArmorPenetration;
            }
        }

        return 0;
    }
    // End mono

    private void OnCartridgeExamine(EntityUid uid, CartridgeAmmoComponent component, ExaminedEvent args)
    {
        if (component.Spent)
        {
            args.PushMarkup(Loc.GetString("gun-cartridge-spent"));
        }
        else
        {
            args.PushMarkup(Loc.GetString("gun-cartridge-unspent"));
        }

        // Mono start
        var ignoreResistancesSpec = GetProjectileIgnoreResistances(component.Prototype); // Mono
        var armorPenetrationSpec = GetProjectileArmorPenetration(component.Prototype); // Mono

        if (ignoreResistancesSpec == true) // Mono
        {
            args.PushMarkup(Loc.GetString("cartridge-full-ap"));
        }
        else if (armorPenetrationSpec != 0)
        {
            args.PushMarkup(Loc.GetString("cartridge-partial-ap",("percent", armorPenetrationSpec * 100)));
        }
        // End Mono
    }
}
