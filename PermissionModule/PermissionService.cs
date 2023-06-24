using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neuron.Core.Meta;
using Neuron.Modules.Configs;
using ReloadModule;
using PluginAPI.Core;

namespace PermissionModule;

public class PermissionService : Service
{
    private ConfigService _configService;
    private ReloadService _reloadService;
    public ConfigContainer Container { get; private set; }
    public Dictionary<string, PermissionGroup> Groups { get; private set; } = new();

    private readonly PermissionGroup _fallBackDefault = new()
    {
        Default = true,
        Permissions = new List<string> { "neuron.help", "neuron.plugin" }
    };

    public PermissionService(ConfigService configService, ReloadService server)
    {
        _reloadService = server;
        _configService = configService;
    }

    public override void Enable()
    {
        _reloadService.Reload.Subscribe(Reload);
        try
        {
            Container = _configService.GetContainer("permissions.syml");
            LoadGroups();
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to load permission.syml\n" + ex);
        }
    }

    public override void Disable()
    {
        _reloadService.Reload.Unsubscribe(Reload);
    }

    public void Reload(ReloadEvent _ = null)
    {
        Container.Load();
        LoadGroups();
    }

    public void Store()
    {
        foreach (var pair in Groups)
        {
            Container.Document.Set(pair.Key, pair.Value);
        }

        Container.Store();
        Reload();
    }

    private PermissionGroup GetGroupInsensitive(string key) => Groups
        .FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)).Value;

    private void LoadGroups()
    {
        var groups = new Dictionary<string, PermissionGroup>();
        foreach (var section in Container.Document.Sections)
        {
            try
            {
                if (groups.ContainsKey(section.Key))
                {
                    Logger.Warn(
                            $"Group {section.Key} was found a second time. Second instance will be skipped");
                    continue;
                }

                var group = section.Value.Export<PermissionGroup>();
                groups[section.Key] = group;
            }
            catch (Exception ex)
            {
                Logger
                    .Error($"Loading of Permissions section {section.Key} failed\n" + ex);
            }
        }

        Groups = groups;

        if (Groups.Count == 0)
        {
            Groups["Owner"] = new PermissionGroup
            {
                Badge = "Owner",
                Color = "rainbow",
                Cover = true,
                Hidden = true,
                KickPower = 254,
                Members = new List<string> { "0000000@steam" },
                Inheritance = new List<string> { "User" },
                Permissions = new List<string> { "*" },
                RemoteAdmin = true,
                RequiredKickPower = 255
            };

            Groups["User"] = new PermissionGroup
            {
                Default = true,
                Permissions = new List<string> { "neuron.help", "neuron.plugin" },
                Members = null,
                Inheritance = null,
            };

            Store();
        }


        foreach (var player in PluginAPI.Core.Player.GetPlayers<PermissionPlayer>())
            player.RefreshPermission(!string.IsNullOrEmpty(player.ReferenceHub.serverRoles.HiddenBadge));
    }

    public PermissionGroup GetDefaultGroup() => Groups.Values.FirstOrDefault(x => x.Default) ?? _fallBackDefault;
    public PermissionGroup GetNorthwoodGroup() => Groups.Values.FirstOrDefault(x => x.NorthWood)?.Copy();

    public bool AddServerGroup(PermissionGroup group, string groupName)
    {
        var current = GetGroupInsensitive(groupName);
        if (current != null) return false;
        Groups[groupName] = group;
        Store();
        return true;
    }

    public bool DeleteServerGroup(string groupName)
    {
        var removed = Groups.Remove(groupName);
        Store();
        return removed;
    }

    public bool ModifyServerGroup(string groupName, PermissionGroup group)
    {
        var current = GetGroupInsensitive(groupName);
        if (current == null) return false;
        Groups[groupName] = group;
        Store();
        return true;
    }

    public PermissionGroup GetServerGroup(string groupName) => GetGroupInsensitive(groupName);

    public PermissionGroup GetPlayerGroup(Player player)
    {
        var group = Groups.Values.FirstOrDefault(x => x.Members != null && x.Members.Contains(player.UserId));

        if (group != null)
            return group.Copy();

        var nwGroup = GetNorthwoodGroup();

        if (player.ReferenceHub.serverRoles.Staff && nwGroup != null)
            return nwGroup;

        return GetDefaultGroup();
    }

    public PermissionGroup GetPlayerGroup(string UserID)
    {
        var group = Groups.Values.FirstOrDefault(x => x.Members != null && x.Members.Contains(UserID));

        if (group != null)
            return group.Copy();

        var nwGroup = GetNorthwoodGroup();

        if (UserID.ToLower().Contains("@northwood") && nwGroup != null)
            return nwGroup;

        return GetDefaultGroup();
    }

    public bool AddPlayerToGroup(string groupName, string userid)
    {
        var group = GetGroupInsensitive(groupName);
        if (group == null) return false;
        if (!userid.Contains("@")) return false;

        RemovePlayerGroup(userid);

        group.Members ??= new List<string>();

        group.Members.Add(userid);
        Store();
        return true;
    }

    public bool RemovePlayerGroup(string userid)
    {
        if (!userid.Contains("@")) return false;
        var doSave = false;
        foreach (var group in Groups.Where(x => x.Value.Members != null && x.Value.Members.Contains(userid)))
        {
            group.Value.Members.Remove(userid);
            doSave = true;
        }
        if (doSave) Store();
        return doSave;
    }
}
