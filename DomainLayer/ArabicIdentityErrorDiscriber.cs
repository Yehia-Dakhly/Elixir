using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    public class ArabicIdentityErrorDiscriber : IdentityErrorDescriber
    {
        // 1. ترجمة خطأ تكرار الإيميل
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"البريد الإلكتروني '{email}' مسجل بالفعل."
            };
        }

        // 2. ترجمة خطأ تكرار اسم المستخدم
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"اسم المستخدم '{userName}' مأخوذ من قبل، حاول استخدام اسم آخر."
            };
        }

        // 3. (اختياري) ترجمة أخطاء الباسورد الشائعة
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"كلمة المرور يجب أن تكون {length} أحرف على الأقل."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "كلمة المرور يجب أن تحتوي على أرقام (0-9)."
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "كلمة المرور يجب أن تحتوي على حرف صغير (a-z)."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "كلمة المرور يجب أن تحتوي على حرف كبير (A-Z)."
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "كلمة المرور يجب أن تحتوي على رمز خاص (مثل @, #, $)."
            };
        }

        // --- 4. أخطاء خاصة بالبيانات (User & Role) ---

        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = $"اسم المستخدم '{userName}' غير صالح (يحتوي على أحرف ممنوعة)."
            };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = $"البريد الإلكتروني '{email}' غير صالح."
            };
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError
            {
                Code = nameof(InvalidRoleName),
                Description = $"اسم الصلاحية '{role}' غير صالح."
            };
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateRoleName),
                Description = $"الصلاحية '{role}' موجودة بالفعل."
            };
        }

        // --- 5. أخطاء تقنية (Concurrency & System) ---

        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            {
                Code = nameof(ConcurrencyFailure),
                Description = "فشلت العملية لأن البيانات تم تعديلها من قبل مستخدم آخر في نفس الوقت."
            };
        }

        public override IdentityError DefaultError()
        {
            return new IdentityError
            {
                Code = nameof(DefaultError),
                Description = "حدث خطأ غير معروف."
            };
        }

        // --- 6. أخطاء إضافية للباسورد (لو مفعل خيار UniqueChars) ---

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUniqueChars),
                Description = $"كلمة المرور يجب أن تحتوي على {uniqueChars} حروف مختلفة على الأقل."
            };
        }
    }
}
