using BookToVoice.Core.TextFilters;
using NUnit.Framework;

namespace BookToVoice.Core.Test.TextFilters
{
    [TestFixture]
    class HeadDeleteFilterTest
    {

        string headString = @"  Жемчужина полнолуния (fb2)

   Просмотреть b281409view
   Читать b281409read

Жемчужина полнолуния b281409 (Гость полнолуния s24581-3)  
(читать) http:anyreads.comread#281409   (скачать)
b281409download - Ярослава Лазарева a60907


      Ярослава Лазарева Жемчужина полнолуния


      Часть первая
      Сны
    Росинка падает с цветка
    Прозрачною слезой.
    Прекрасен мир… Но без тебя
    Наполнен он тоской.
    Все плачет, кажется, скорбя:
    Росинки, капельки дождя…
    Мир без тебя – пустой.
        Григорий Грег";


        [Test]
        public void HeadDeleteTest()
        {
            var filter = new HeadDeleteFilter("(\\d+)(download)");

            var outstr = filter.Execute(headString);
            Assert.False(outstr.Contains("Гость полнолуния"));
        }
    }
}
