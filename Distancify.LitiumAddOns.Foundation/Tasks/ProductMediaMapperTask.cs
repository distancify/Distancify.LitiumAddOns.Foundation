using Distancify.LitiumAddOns.Foundation;
using Distancify.LitiumAddOns.ProductMedia.Mapping;

namespace Distancify.LitiumAddOns.Tasks
{
    public class ProductMediaMapperTask : NonConcurrentTask
    {
        private readonly IMediaMapper _productFileMapper;

        public ProductMediaMapperTask(IMediaMapper productFileMapper)
        {
            _productFileMapper = productFileMapper;
        }
        
        protected override void Run()
        {
            _productFileMapper.Map();
        }
    }
}