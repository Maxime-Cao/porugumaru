using Puroguramu.Domains;

namespace Puroguramu.Infrastructures.Data;

public class GroupsRepository : IGroupsRepository
{
    private readonly PuroguramuDbContext _puroguramuDbContext;

    public GroupsRepository(PuroguramuDbContext puroguramuDbContext)
    {
        _puroguramuDbContext = puroguramuDbContext;
    }

    public IEnumerable<GroupLab> GetAllGroups()
    {
        var dbGroups = _puroguramuDbContext.Groups.OrderBy(g => g.GroupName).ToList();
        var groups = new List<GroupLab>();

        foreach (var group in dbGroups)
        {
            groups.Add(new GroupLab() {IdGroup = group.IdGroup, GroupName = group.GroupName});
        }

        return groups;
    }
}
