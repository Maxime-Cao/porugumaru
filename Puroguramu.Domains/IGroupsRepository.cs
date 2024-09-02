namespace Puroguramu.Domains;

public interface IGroupsRepository
{
    IEnumerable<GroupLab> GetAllGroups();
}
