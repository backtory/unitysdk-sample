<div dir=rtl>

	
# پروژه‌ی نمونه‌ی SDK یونیتی بکتوری

در این پروژه مشاهده می‌کنید که چگونه می‌توان از SDK یونیتی mBaaS
[بکتوری](https://backtory.com)
استفاده کرد و یک اپلیکیشن ساخت.
این پروژه توسط توسعه‌دهندگان SDK بکتوری به‌روزرسانی می‌شود.

## نحوه‌ی build گرفتن

برای اجرای این پروژه کافیست آن را به کمک Unity باز کرده، سپس از آن یک apk خروجی گرفته و روی دستگاه خود نصب کنید
تا سرویس‌های SDK بکتوری را در عمل مشاهده کنید.

برای اطلاعات بیشتر درباره‌ی نحوه‌ی نصب و استفاده از SDK یونیتی بکتوری
می‌توانید به
[مستندات](https://backtory.com/documents/intro/setup/unity.html)
بکتوری سر بزنید.

## نحوه‌ی کار با اپلیکیشن

بعد از اجرای اپلیکیشن بر روی device مشاهده خواهید کرد
که این برنامه از tab های مختلفی تشکیل شده است و
هر tab شامل دکمه‌هایی است که توابع مربوط به یک سرویس خاص بکتوری را اجرا می‌کنند.
در ادامه عملکرد دکمه‌ها و توابع را گام‌به‌گام توضیح می‌دهیم:

### سرویس کاربران (Auth)

1. Register: یک کاربر را با نام کاربری، رمز عبور و ایمیل تصادفی می‌سازد.
2. Login: کابر ثبت‌نام شده در قسمت قبل را لاگین می‌کند.
3. Guest Login: به صورت کاربر مهمان لاگین می‌کند.
4. Cmplt Guest: فرآیند ثبت نام کاربر مهمان را تکمیل می‌کند.
 در صورتی می‌توانید از این دکمه استفاده کنید که قبل از آن به صورت کاربر مهمان لاگین کرده باشید.
5. Change Pass: درخواست تغییر رمز عبور را به سرور بکتوری ارسال می‌کند.
6. Forget pass: اعلام فراموشی رمز عبور کرده و از سرور درخواست رمز عبور جدید می‌کند.
7. Current User: کاربری که در حال حاضر لاگین کرده را نشان می‌دهد.
8. Update User: کاربری که در حال حاضر لاگین است، را با پارامترهای مشخصی به‌روزرسانی می‌کند.
9. Logout: لاگ اوت می‌کند.



### سرویس رایانش (Cloudcode)

1. Echo: تابعی روی سرویس رایانش به نام
Echo
را فراخوانی می‌کند. این تابع دسترسی Public دارد،
 یعنی بدون این‌که کاربری لاگین باشد، می‌تواند اجرا شود.
2. Search: تابعی روی سرویس رایانش به نام
Search
را فراخوانی می‌کند. این تابع دسترسی Authenticated دارد،
 یعنی برای اجرای آن حتما باید کاربری لاگین باشد.



### سرویس مرکز بازی (Game)

1. Send Event: رویدادی شامل دو فیلد «سکه» و«زمان» را به سرور ارسال می‌کند. مقادیر سکه و زمان را می‌توانید در دو input field در بالای همین دکمه وارد کنید.
  دقت کنید که اگر لاگین نباشید، صدا زدن این متد با exception همراه است.
2. My Rank: رتبه‌ی کاربر حاضر را در لیدربورد مشخصی نشان می‌دهد.
3. Top Players: بازیکنان برتر لیدربورد مشخصی را برمی‌گرداند.
4. Around Me: بازیکنان اطراف کاربری که لاگین کرده است، در لیدربورد را نشان می‌دهند.



### سرویس فایل (Storage)

1. Upload:  یک فایل را با محتوای داده شده در مسیر داده شده آپلود می‌کند.
3. Rename: یک فایل را که در سمت سرور ذخیره شده است، تغییر نام می‌دهد.
4. Delete: فایلی که در مسیر داده شده قرار دارد، را حذف می‌کند.


### سرویس پایگاه داده (Database)

1. Save Note: یک موجودیت از جنس Note را با مشخصات داده شده در جدولی به همین نام ایجاد می‌کند.
2. Delete Note: همان Note ای را که پیش از این ساخته بود، حذف می‌کند.
3. All Pinned Notes: یک کوئری به سرویس DB می‌زند
و همه‌ی نوت‌هایی که مقدار pinned شان برابر true هست، را باز می‌گرداند.
4. Notes with title containing todo: یک کوئری به سرویس DB می‌زند
 و همه‌ی نوت‌هایی که title شان شامل رشته‌ی todo هست، را باز می‌گرداند.




### برگه‌ی انطباق (Matchmaking)

1. Login mm user 1: کاربری با نام کاربری
testUser
 را لاگین می‌کند. این کاربر برای تست مچ میکینگ به کار می‌آید
  و سایر مشخصات آن در فایل TestUser.java قابل مشاهده است.
2. login mm user 2: کاربری با نام کاربری
testUser2
را لاگین می‌کند. این کاربر برای تست مچ میکینگ به کار می‌آید
 و سایر مشخصات آن در فایل TestUser.java قابل مشاهده است.
3. Realtime Connect: اتصال به سرویس بلادرنگ را برقرار می‌کند.
4. Realtime Disconnect: اتصال به سرویس بلادرنگ را قطع می‌کند.
5. Request Match: درخواست انطباق را به سرور ارسال می‌کند.
6. Cancel Request: در صورتی که پیش از این درخواستی به سرور ارسال کرده باشد،
 این درخواست را کنسل می‌کند (درخواست کنسل کردن را به سرور ارسال می‌کند.)



### برگه‌ی چالش (Challenge)

1. Login challenge user 1: کاربری با نام کاربری testUser را لاگین می‌کند. این کاربر برای تست چالش به کار می‌آید و سایر مشخصات آن در فایل TestUser.java قابل مشاهده است.
2. login challenge user 2: کاربری با نام کاربری
testUser2
را لاگین می‌کند. این کاربر برای تست مچ میکینگ به کار می‌آید
 و سایر مشخصات آن در فایل TestUser.java قابل مشاهده است.
3. Realtime Connect: اتصال به سرویس بلادرنگ را برقرار می‌کند.
4. Realtime Disconnect: اتصال به سرویس بلادرنگ را قطع می‌کند.
5. Request Challenge: در صورتی که کاربر لاگین کرده testUser1 باشد، درخواست چالشی به testUser2 ارسال می‌کند. در غیر این ‌صورت به testUser1 درخواست چالش ارسال می‌کند.
6. Cancel Challenge: در صورتی که پیش از این درخواست چالشی کرده باشید، آن درخواست را کنسل می‌کند.
7. Accept Challenge: در صورتی که برای کاربر لاگین کرده در اپ درخواست چالشی ارسال شده باشد، آن درخواست را می‌پذیرد.
7. Decline Challenge: در صورتی که برای کاربر لاگین کرده در اپ درخواست چالشی ارسال شده باشد، آن درخواست را رد می‌کند.
8. Request Active Challenges: لیست چالش‌های فعال را نشان می‌دهد.


### برگه‌ی بلادرنگ (Realtime)

برای این‌که بتوانید یک بازی ریل‌تایم را بین دو نفر آغاز کنید،
 می‌توانید از درخواست انطباق یا چالش شروع کنید.
برای این‌که با درخواست انطباق به یک بازی بلادرنگ برسید، مراحل زیر را طی کنید:
1. اپلیکیشن را بر روی دو دستگاه اندرویدی نصب نمایید.
2. در برگه‌ی matchmaking، بر روی یک دستگاه دکمه‌ی login mm user 1
و بر روی دستگاه دیگر دکمه‌ی
login mm user 2
را صدا کنید.
3. در هر دو دستگاه Realtime Connect کنید.
4. در هر دو دستگاه Request Match کنید.
5. اکنون لیسنرهای onMatchUpdated
و سپس
onMatchFound
صدا زده می‌شوند و بازی بلادرنگ بر اساس انطباق ساخته شده ایجاد می‌گردد.

هم‌چنین، برای این‌که با درخواست چالش به یک بازی بلادرنگ برسید، مراحل زیر را طی کنید:
1. اپلیکیشن را بر روی دو دستگاه اندرویدی نصب نمایید.
2. در برگه‌ی challenge، بر روی یک دستگاه دکمه‌ی login challenge user 1
و بر روی دستگاه دیگر دکمه‌ی
login challenge user 2
را صدا کنید.
3. در هر دو دستگاه Realtime Connect کنید.
4. در یکی از دستگاه‌ها Request Challenge را فشار دهید.
5. در دستگاه دیگر مشاهده خواهید کرد که کاربر به چالشی دعوت شده است.
روی Accept Challenge فشار دهید.
6. اکنون لیسنر
onChallengeReady
صدا زده می‌شود و بازی بلادرنگ بر اساس انطباق ساخته شده ایجاد می‌گردد.


  اکنون می‌توانید با برگه‌ی بلادرنگ آغاز به کار کنید.

1. Connect to Match: به انطباق پیدا شده وصل می‌شود. در صورتی که طرفین بازی به این match وصل شوند،
 لیسنر onMatchStartedMessage صدا زده می‌شود.
2. Send Event: یک رویداد بلادرنگ ساده ارسال می‌کند.
 این رویداد هم روی دستگاه شما و هم دستگاه حریف دریافت می‌شود.
3. Disconnect from match: از بازی بلادرنگ خارج می‌شود.
4. Direct Message: یک پیام مستقیم به حریف بر روی بستر بازی بلادرنگ ارسال می‌کند.
5. Send Chat To Match: یک پیام به همه‌ی بازیکنان درگیر در بازی بلادرنگ ارسال می‌کند.
6. Send Match Result: برندگان بازی را به سرور ارسال می‌کند.




### برگه‌ی خرید درون برنامه‌ای (Inapp Purchase)

1. Get Sku Details: جزئیات محصولات قابل خرید (و نه قابل اشتراک) را از کافه‌بازار دریافت کرده
 و نمایش می‌دهد.
2. Get Purchases: لیست محصولات خریداری شده از کافه‌بازار را نشان می‌دهد.
 در صورتی که Dropdown مربوطه روی گزینه‌ی inapp یا subs باشد،
  به ترتیب محصولات خریداری شده‌ی خریدنی یا اشتراکی را نشان می‌دهد.
3. Purchase Item: کالای gas را به قیمت صفر ریال خریداری می‌کند.
4. Consume Item: کالای خریداری شده در قسمت قبل را مصرف می‌کند.
5. Upgrade to premium: کالای premium را به قیمت صفر ریال خریداری می‌کند.
 دقت کنید که این کالا غیر مصرفی است؛ یعنی،
  برنامه طوری طراحی شده که این کالا را مصرف نکند.
6. Subscribe: به کالای infinite_gas به مدت یک ماه اشتراک می‌یابد.

در هنگام استفاده از دکمه‌های 3 تا 6 می‌توانید
با انتخاب حالت‌های secure یا insecure، مدل امنیتی خود را مشخص نمایید.


</div>
