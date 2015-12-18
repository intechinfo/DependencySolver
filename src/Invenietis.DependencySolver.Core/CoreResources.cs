namespace Invenietis.DependencySolver.Core
{
    static class CoreResources
    {
        internal static readonly string MustBeNotNullNorWhiteSpace = "The {0} must be not null nor whitespace.";

        internal static readonly string TagMustBeValid = "The release tag must be valid.";

        internal static readonly string InvalidProjectType = "The project type is not valid.";

        internal static readonly string ProjectBelongsToAnotherContext = "The project belongs to another context.";

        internal static readonly string PackageBelongsToAnotherContext = "The nuget package belongs to another context.";

        internal static readonly string SolutionBelongsToAnotherContext = "The solution belongs to another context.";
    }
}