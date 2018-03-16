using CoolestLibrary;
using Ninject;
using NUnit.Framework;

namespace CoolestLibraryTests
{
    [TestFixture]
    public class KernelTests
    {
        [Test]
        public void BindWhenClassHas_DefaultAttributeCtor_TypesMatch()
        {
            // assemble
            var k = new MyTestKernel();
            var root = k.Get<Conservatory>();

            // act
            var artist = root.ArtistFactory(2);

            // assert
            Assert.IsInstanceOf<Actor>(artist);
        }

        [Test]
        public void BindWhenClassHas_NonDefaultAttributeCtor_TypesMatch()
        {
            // assemble
            var k = new MyTestKernel();
            var root = k.Get<Conservatory>();

            // act
            var teacher = root.TeacherFactory("Dumbledore");

            // assert
            Assert.IsInstanceOf<Professor>(teacher);
        }

    }
}
