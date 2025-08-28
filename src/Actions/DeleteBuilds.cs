using System.Text.RegularExpressions;
using AzdTool.Extensions;
using AzdTool.Visitors;
using Microsoft.TeamFoundation.Build.WebApi;
using AZBuild = Microsoft.TeamFoundation.Build.WebApi.Build;

namespace AzdTool.Actions;

internal class DeleteBuilds(VisitorNode node) : BatchOrUnitAction<AZBuild>(node, "build", "delete")
{
	protected override bool Filter(AZBuild item, string filter) => Regex.IsMatch(item.GetFormattedTitle(), filter, RegexOptions.IgnoreCase);

	protected override async Task ActionAsync(IEnumerable<AZBuild> items)
	{
		var organization = Node.Ancestor<Organization>();
		await DeleteBuildsAsync(organization, items);
	}

	public static async Task DeleteBuildsAsync(Organization organization, IEnumerable<AZBuild> items)
	{
		await organization.ExecuteClientAsync<BuildHttpClient>(async client =>
		{
			foreach (var item in items)
			{
				var projectId = item.Project.Id;
				var buildId = item.Id;

				var leases = await client.GetRetentionLeasesForBuildAsync(projectId, buildId);
				if (leases.Count > 0)
				{
					await client.DeleteRetentionLeasesByIdAsync(projectId, leases.Select(l => l.LeaseId));
				}

				await client.DeleteBuildAsync(projectId, buildId);
			}
		});
	}
}
