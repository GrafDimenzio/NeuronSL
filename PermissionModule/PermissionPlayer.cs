using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Core;
using PluginAPI.Core.Interfaces;
using PluginAPI.Core.Factories;
using Neuron.Core;

namespace PermissionModule;

public class PermissionPlayer : Player
{
    public PermissionPlayer(IGameComponent component) : base(component) { }

    public PermissionGroup PermissionGroup => Globals.Get<PermissionService>().GetPlayerGroup(UserId);

    public void RefreshPermission(bool display) => RefreshPermission(display, PermissionGroup);

    public void RefreshPermission(bool display, PermissionGroup pmGroup)
    {
        var group = new UserGroup
        {
            BadgeText = pmGroup.Badge.ToUpper() == "NONE" ? null : pmGroup.Badge,
            BadgeColor = pmGroup.Color.ToUpper() == "NONE" ? null : pmGroup.Color,
            Cover = pmGroup.Cover,
            HiddenByDefault = pmGroup.Hidden,
            KickPower = pmGroup.KickPower,
            Permissions = pmGroup.GetVanillaPermissionValue(),
            RequiredKickPower = pmGroup.RequiredKickPower,
            Shared = false
        };

        var globalAccessAllowed = true;

        if (GlobalPerms != 0 && globalAccessAllowed)
            group.Permissions |= GlobalPerms;


        ReferenceHub.serverRoles.Group = group;
        ReferenceHub.serverRoles.Permissions = group.Permissions;
        RAAccess = pmGroup.RemoteAdmin || GlobalRemoteAdmin;
        ReferenceHub.serverRoles.AdminChatPerms = PermissionsHandler.IsPermitted(group.Permissions, PlayerPermissions.AdminChat);
        ReferenceHub.serverRoles._badgeCover = group.Cover;
        ReferenceHub.queryProcessor.GameplayData = PermissionsHandler.IsPermitted(group.Permissions, PlayerPermissions.GameplayData);

        ReferenceHub.serverRoles.SendRealIds();

        if (string.IsNullOrEmpty(group.BadgeText))
        {
            ReferenceHub.serverRoles.SetColor(null);
            ReferenceHub.serverRoles.SetText(null);
            if (!string.IsNullOrEmpty(ReferenceHub.serverRoles.PrevBadge))
            {
                ReferenceHub.serverRoles.HiddenBadge = ReferenceHub.serverRoles.PrevBadge;
                ReferenceHub.serverRoles.GlobalHidden = true;
                ReferenceHub.serverRoles.RefreshHiddenTag();
            }
        }
        else
        {
            if (ReferenceHub.serverRoles._hideLocalBadge || (group.HiddenByDefault && !display && !ReferenceHub.serverRoles._neverHideLocalBadge))
            {
                ReferenceHub.serverRoles.Network_myText = null;
                ReferenceHub.serverRoles.Network_myColor = "default";
                ReferenceHub.serverRoles.HiddenBadge = group.BadgeText;
                ReferenceHub.serverRoles.RefreshHiddenTag();
                ReferenceHub.serverRoles.TargetSetHiddenRole(Connection, group.BadgeText);
            }
            else
            {
                ReferenceHub.serverRoles.HiddenBadge = null;
                ReferenceHub.serverRoles.RpcResetFixed();
                ReferenceHub.serverRoles.Network_myText = group.BadgeText;
                ReferenceHub.serverRoles.Network_myColor = group.BadgeColor;
            }
        }

        var flag = ReferenceHub.serverRoles.Staff ||
                   PermissionsHandler.IsPermitted(group.Permissions, PlayerPermissions.ViewHiddenBadges);
        var flag2 = ReferenceHub.serverRoles.Staff ||
                    PermissionsHandler.IsPermitted(group.Permissions, PlayerPermissions.ViewHiddenGlobalBadges);
        if (flag || flag2)
            foreach (var player in PluginAPI.Core.Player.GetPlayers<PermissionPlayer>())
            {
                if (!string.IsNullOrEmpty(player.ReferenceHub.serverRoles.HiddenBadge) &&
                    (!player.ReferenceHub.serverRoles.GlobalHidden || flag2) && (player.ReferenceHub.serverRoles.GlobalHidden || flag))
                    player.ReferenceHub.serverRoles.TargetSetHiddenRole(Connection, player.ReferenceHub.serverRoles.HiddenBadge);
            }
    }

    public bool HasPermission(string permission) => IsServer || PermissionGroup.HasPermission(permission);

    /// <summary>
    /// True if the Player can Open the RemoteAdmin
    /// </summary>
    public bool RAAccess
    {
        get => ReferenceHub.serverRoles.RemoteAdmin;
        set
        {
            if (value)
                RaLogin();
            else
                RaLogout();
        }
    }

    /// <summary>
    /// Gives the Player access to the RemoteAdmin (doesn't automatically give any Permissions)
    /// </summary>
    public void RaLogin()
    {
        ReferenceHub.serverRoles.RemoteAdmin = true;
        ReferenceHub.serverRoles.Permissions = PermissionGroup.GetVanillaPermissionValue() | ReferenceHub.serverRoles._globalPerms;
        ReferenceHub.serverRoles.RemoteAdminMode = ReferenceHub.serverRoles.RemoteAdminMode == ServerRoles.AccessMode.GlobalAccess ? ServerRoles.AccessMode.GlobalAccess : ServerRoles.AccessMode.PasswordOverride;
        if (!ReferenceHub.serverRoles.AdminChatPerms)
            ReferenceHub.serverRoles.AdminChatPerms = PermissionGroup.HasVanillaPermission(PlayerPermissions.AdminChat);
        ReferenceHub.serverRoles.TargetOpenRemoteAdmin(false);
        ReferenceHub.queryProcessor.SyncCommandsToClient();
    }

    /// <summary>
    /// Removes the Player access to the RemoteAdmin
    /// </summary>
    public void RaLogout()
    {
        ReferenceHub.serverRoles.RemoteAdmin = false;
        ReferenceHub.serverRoles.RemoteAdminMode = ServerRoles.AccessMode.LocalAccess;
        ReferenceHub.serverRoles.TargetCloseRemoteAdmin();
    }

    public ulong GlobalPerms => ReferenceHub.serverRoles._globalPerms;

    public bool GlobalRemoteAdmin => ReferenceHub.serverRoles.RemoteAdminMode == ServerRoles.AccessMode.GlobalAccess;
}

public class PermissionPlayerFactory : PlayerFactory
{
    public override Type BaseType => typeof(PermissionPlayer);

    public override IPlayer Create(IGameComponent component) => new PermissionPlayer(component);
}
