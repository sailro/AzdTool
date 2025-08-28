using AzdTool.Extensions;
using AzdTool.Actions;
using AzdTool.Nodes;
using AZBuild = Microsoft.TeamFoundation.Build.WebApi.Build;

namespace AzdTool.Visitors;

internal class Build(Builds builds, AZBuild build) : UnitVisitorNode<AZBuild>
{
	public override AZBuild Item => build;
	public override string Title => build.GetFormattedTitle();
	public override VisitorNode Parent => builds;

	public override List<INode> Children => [
		new CancelBuilds(this),
		new DeleteBuilds(this),
		new GoBack(builds),
		new Exit()
	];
}
