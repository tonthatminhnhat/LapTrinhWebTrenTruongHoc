using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020493.DomainModels
{
    public static class Constants
    {
        public const int ORDER_INIT = 1; //đặt hàng ban đầu
        public const int ORDER_ACCEPTED = 2;//đơn dc chấp nhận
        public const int ORDER_SHIPPING = 3;// đang vận chuyển
        public const int ORDER_FINISHED = 4;// đã hoàn thành
        public const int ORDER_CANCEL = -1;// bị hủy
        public const int ORDER_REJECTED = -2;// bị từ chối
    }
}
