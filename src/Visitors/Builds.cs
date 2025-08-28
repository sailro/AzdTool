using AzdTool.Actions;
using AzdTool.Nodes;
using Microsoft.TeamFoundation.Build.WebApi;
using AZBuild = Microsoft.TeamFoundation.Build.WebApi.Build;

namespace AzdTool.Visitors;

internal class Builds(Pipeline pipeline) : BatchVisitorNode<AZBuild>
{
	public override List<AZBuild> AllItems { get; } = [];
	public override VisitorNode Parent => pipeline;

	public override List<INode> Children
	{
		get
		{
			var result = new List<INode>();

			result.AddRange(AllItems.Select(build => new Build(this, build)));
			result.Add(new CancelBuilds(this));
			result.Add(new DeleteBuilds(this));
			result.Add(new Refresh(this));
			result.Add(new GoBack(pipeline));
			result.Add(new Exit());

			return result;
		}
	}

	public override async Task<INode> ExecuteAsync()
	{
		await FetchNotifyAsync(async ctx =>
		{
			var project = Ancestor<Project>();
			var organization = project.Ancestor<Organization>();

			await organization.ExecuteClientAsync<BuildHttpClient>(async client =>
			{
				AllItems.Clear();
				AllItems.AddRange((await client.GetBuildsAsync(project.Item.Id, [pipeline.Item.Id]))
					.OrderByDescending(build => build.QueueTime));
			});

			ctx.Refresh();
		});

		return await base.ExecuteAsync();
	}
}
