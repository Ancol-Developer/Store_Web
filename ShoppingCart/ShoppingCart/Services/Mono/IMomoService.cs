using ShoppingCart.Models;
using ShoppingCart.Models.Momo;

namespace ShoppingCart.Services.Mono;

public interface IMomoService
{
	Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInforModel model);
	MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
}
