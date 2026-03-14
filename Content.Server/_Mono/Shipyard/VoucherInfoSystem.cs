using Content.Shared._NF.Shipyard;
using Content.Shared._NF.Shipyard.Components;
using Content.Server._NF.Shipyard.Components;
using Content.Shared._Mono.Ships.Components;
using Content.Shared.Examine;
using Robust.Shared.Timing;

namespace Content.Server._Mono.Shipyard;

public class VoucherInfoSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    protected virtual void InitializeVoucher()
    {
        base.Initialize();

        SubscribeLocalEvent<ShipyardVoucherComponent, ExaminedEvent>(OnVoucherExamine);
    }
    private void OnVoucherExamine(EntityUid uid, ShipyardVoucherComponent component, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        if (component.DestroyOnEmpty != true)
        {
            args.PushMarkup(Loc.GetString("voucher-current-redemptions", ("count", component.RedemptionsLeft)));
        }

        var remainingTime = component.NextBuyAt - _timing.CurTime;

        if (remainingTime >= TimeSpan.FromSeconds(0))
        {
            args.PushMarkup(Loc.GetString("voucher-current-cooldown", ("cooldown", Math.Round(remainingTime.TotalSeconds))));
        }
    }
}
