﻿using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Spectre.Console;

namespace AzdTool.Extensions;

internal static class TaskAgentJobRequestExtensions
{
	public static string GetFormattedTitle(this TaskAgentJobRequest taskAgentJobRequest)
		=> $"Job {taskAgentJobRequest.RequestId} - {taskAgentJobRequest.Definition.Name} - {taskAgentJobRequest.Owner.Name}".EscapeMarkup();
}
