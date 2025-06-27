using Microsoft.AspNetCore.Mvc;

namespace FeatureFlagDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("feature-a/variant1")]
    [ServiceFilter(typeof(FeatureFlagVariantFilter))] // 필터를 서비스로 적용, 필요 시 추가
    [FeatureFlagVariant("FeatureA", "Variant1")]
    public IActionResult GetFeatureAVariant1()
    {
        return Ok("Feature A with Variant1 is enabled!");
    }

    [HttpGet("feature-a/variant2")]
    [ServiceFilter(typeof(FeatureFlagVariantFilter))]
    [FeatureFlagVariant("FeatureA", "Variant2")]
    public IActionResult GetFeatureAVariant2()
    {
        return Ok("Feature A with Variant2 is enabled!");
    }

    [HttpGet("feature-c/variant2")]
    [FeatureFlagVariant("FeatureC", "Variant2")]
    public IActionResult GetFeatureCVariant2()
    {
        return Ok("Feature C with Variant2 is enabled!");
    }

    [HttpGet("feature-d")]
    [FeatureFlag("uwo-can-change-name")]
    public IActionResult GetFeatureD()
    {
        return Ok(new { 
            message = "Feature D is enabled!",
            status = "success",
            timestamp = DateTime.UtcNow,
        });        
    }
}
