using EmailCollector.Domain.Entities;

namespace EmailCollector.Api.Controllers.RouteConsts;

public class Routes
{
    /* -------- Base routes -------- */
    private const string Base = "/api";
    private const string ExportsBase = "/export";
    private const string FormsBase = "/forms";
    private const string EmailSignupsBase = "/submissions";
    
    /* -------- Signup forms routes -------- */
    /// <summary>
    /// <see cref="SignupFormsController"/>
    /// </summary>
    public const string SignupFormsControllerBase = Base + FormsBase;
    public const string GetSubmissions = "/submissions";
    public const string Export = "/export";
    
    /* -------- Exports routes -------- */
    /// <summary>
    /// Export is a action performed on <see cref="SignupForm"/>
    /// hence we use the same base route as <see cref="SignupFormsControllerBase"/>
    /// <see cref="ExportsController"/>
    /// </summary>
    public const string ExportsControllerBase = Base + FormsBase;
    
    /* -------- EmailSignups routes -------- */
    public const string EmailSignupsControllerBase = Base + EmailSignupsBase;
    public const string Confirmations = "/confirmations";
    
    /* -------- CustomTemplates routes -------- */
    /// <summary>
    /// <see cref="CustomEmailTemplate"/> is a sub-entity of <see cref="SignupForm"/>
    /// hence we use the same base route as <see cref="SignupFormsControllerBase"/>
    /// </summary>
    public const string CustomTemplatesControllerBase = Base + FormsBase;
    public const string Templates = "/templates";
}