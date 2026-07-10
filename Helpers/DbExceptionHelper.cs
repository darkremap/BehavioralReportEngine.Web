using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BehavioralReportEngine.Web.Helpers
{
    public static class DbExceptionHelper
    {
        public static string GetFriendlyMessage(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    case 547 when sqlEx.Message.Contains("CHECK constraint"):
                        return "مقدار وارد شده با محدودیت‌های مجاز این فیلد مطابقت ندارد. لطفاً مقادیر فرم را بررسی کنید.";
                    case 547:
                        return "این عملیات امکان‌پذیر نیست چون رکورد به داده‌های مرتبط دیگری وابسته است.";
                    case 2601:
                    case 2627:
                        return "مقدار وارد شده تکراری است و قبلاً در سیستم ثبت شده.";
                    default:
                        return "خطایی هنگام ذخیره‌سازی اطلاعات رخ داد. لطفاً مقادیر وارد شده را بررسی کنید.";
                }
            }
            return "خطایی هنگام ذخیره‌سازی اطلاعات رخ داد.";
        }
    }
}
