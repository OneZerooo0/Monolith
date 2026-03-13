using Content.Server._NF.Shipyard;
//using Content.Server._NF.Shipyard.Components;
using Content.Shared._NF.Shipyard;
using Content.Shared._Mono.Ships.Components;
using Content.Shared.Examine;
using Robust.Shared.Timing;

namespace Content.Shared._Mono.Shipyard;

public class VoucherInfoSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;

    protected const string RedemptionsExamineColor = "yellow";
    protected const string CooldownExamineColor = "yellow";

    protected virtual void InitializeVoucher()
    {
        SubscribeLocalEvent<ShipyardVoucherComponent, ExaminedEvent>(OnVoucherExamine);
    }
    private void OnVoucherExamine(EntityUid uid, ShipyardVoucherComponent component, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        if (component.DestroyOnEmpty != true)
        {
            args.PushMarkup(Loc.GetString("voucher-current-redemptions", ("color", RedemptionsExamineColor), ("count", component.RedemptionsLeft)));
        }

        var remainingTime = component.NextBuyAt - _timing.CurTime;

        if (remainingTime >= TimeSpan.FromSeconds(0))
        {
            args.PushMarkup(Loc.GetString("voucher-current-cooldown", ("color", CooldownExamineColor), ("cooldown", Math.Round(remainingTime.TotalSeconds))));
        }
    }
}
