using System;
using System.Diagnostics;
using NUnit.Framework;
using gdio.unity_api;
using gdio.unity_api.v2;
using gdio.common.objects;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Media;
using System.Collections.Generic;

namespace IntegrationTests
{
    [TestFixture]
    public class GameplayFunctionalityAndLevelProgression
    {

        public string testMode = TestContext.Parameters.Get("Mode", "IDE");
        public string pathToExe = TestContext.Parameters.Get("pathToExe", null);

        ApiClient api;

        [OneTimeSetUp]
        public void Connect()
        {
            try
            {

                api = new ApiClient();
                if (pathToExe != null)
                {
                    ApiClient.Launch(pathToExe);
                    api.Connect("localhost", 19734, false, 30);
                }

                else if (testMode == "IDE")
                {
                    api.Connect("localhost", 19734, true, 30);
                }

                else api.Connect("localhost", 19734, false, 30);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            api.EnableHooks(HookingObject.ALL);

            //Start the Game 
            api.WaitForObject("//*[@name='StartButton']");
            api.ClickObject(MouseButtons.LEFT, "//*[@name='StartButton']", 30);
            api.Wait(3000);

        }


        [Test, Order(0)]
        public void ReachingZone2()
        {
            Vector3 infosign = api.GetObjectFieldValue<Vector3>("//*[@name='InfoPost'][1]/fn:component('UnityEngine.Transform')/@position");
            api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('UnityEngine.Transform')", "position", infosign);
            Vector3 heroPos = api.GetObjectFieldValue<Vector3>("//*[@name='Ellen']/fn:component('UnityEngine.Transform')/@position");
            api.Wait(2000);
            api.KeyPress(new KeyCode[] { KeyCode.S }, 100);
            api.Wait(100);
            api.KeyPress(new KeyCode[] { KeyCode.Space }, 100);
            api.Wait(100);

            Assert.That(heroPos.x, Is.EqualTo(infosign.x), "Hero didn't reach desired position");

        }


        [Test, Order(1)]
        public void CollectingTheFirstKey()
        {
            api.Wait(3500);
            Vector3 keycollect = api.GetObjectPosition("(//*[@name='Key'])");
            keycollect.y += 3;
            Console.WriteLine(keycollect.ToString());

            //Found this solution within the documentation. It seems to be the only viable approach for determining if the key has been picked up.
            //Console.WriteLine(api.GetObjectFieldValue<Color>("/*[@name='KeyCanvas']/*[@name='KeyIcon(Clone)'][0]/*[@name='Key']/fn:component('UnityEngine.UI.Image')/@color").ToString().Equals("RGBA(1.000, 1.000, 1.000, 1.000)"));

            api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('UnityEngine.Transform')", "position", keycollect);
            api.Wait(3500);


            Vector3 position1 = api.GetObjectPosition("//*[@name='Ellen']");
            position1.x += 6;
            api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('UnityEngine.Transform')", "position", position1);
            api.KeyPress(new KeyCode[] { KeyCode.D }, 500);

            //Look at the comment above
            Color expectedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            Color actualColor = api.GetObjectFieldValue<Color>("/*[@name='KeyCanvas']/*[@name='KeyIcon(Clone)'][0]/*[@name='Key']/fn:component('UnityEngine.UI.Image')/@color");

            Assert.That(expectedColor.Equals(actualColor));
        }


        [Test, Order(2)]
        public void ReachingZone3()
        {
            api.Wait(400);
            api.WaitForObject("//*[@name='WeaponPickup']");
            api.LoadScene("Zone3");

            Assert.That(api.GetSceneName(), Is.EqualTo("Zone3"), "Player is not in Zone3");
        }


        [Test, Order(3)]
        public void PickingUpTheWeapon()
        {
            api.Wait(5000);
            Vector3 weaponPickup = api.GetObjectFieldValue<Vector3>("//*[@name='WeaponPickup']/fn:component('UnityEngine.Transform')/@localPosition");
            api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('UnityEngine.Transform')", "position", weaponPickup);
        }


        [Test, Order(4)]
        public void DestroyingTheFirstColumn()
        {
            api.Wait(1000);
            api.KeyPress(new KeyCode[] { KeyCode.A }, 100);
            api.Wait(850);
            api.KeyPress(new KeyCode[] { KeyCode.K }, 100);
            api.Wait(100);

        }


        [Test, Order(5)]
        public void DestroyingTheSecondColumn()
        {
            api.Wait(5000);
            Vector3 seconddestructibleColumn = api.GetObjectFieldValue<Vector3>("//*[@name='DestructableColumn']/fn:component('UnityEngine.Transform')/@localPosition");

            Console.WriteLine(seconddestructibleColumn);
            seconddestructibleColumn.x += 1;
            Console.WriteLine(seconddestructibleColumn);
            api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('UnityEngine.Transform')", "position", seconddestructibleColumn);
            api.KeyPress(new KeyCode[] { KeyCode.K }, 100);
            api.Wait(100);
        }


        [Test, Order(6)]
        public void ShootingASnail()
        {
            api.Wait(1000);
            Vector3 plant_position = api.GetObjectPosition("(//*[@name='LowerPlants3_14 (1)'])");
            api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('UnityEngine.Transform')", "position", plant_position);
            api.KeyPress(new KeyCode[] { KeyCode.S }, 100);
            api.KeyPress(new KeyCode[] { KeyCode.O }, 100);
            api.Wait(100);
        }


        [Test, Order(7)]
        public void CollectingTheSecondKey()
        {
            api.Wait(5000);
            Vector3 keycollect = api.GetObjectPosition("(//*[@name='Key'])");
            keycollect.y += 3;
            Console.WriteLine(keycollect.ToString());


            api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('UnityEngine.Transform')", "position", keycollect);
            api.Wait(3000);
            Color expectedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            Color actualColor = api.GetObjectFieldValue<Color>("/*[@name='KeyCanvas']/*[@name='KeyIcon(Clone)'][1]/*[@name='Key']/fn:component('UnityEngine.UI.Image')/@color");

            Assert.That(expectedColor.Equals(actualColor));
        }


        [Test, Order(8)]
        public void ReturningToZone2()
        {

            api.Wait(100);
            api.KeyPress(new KeyCode[] { KeyCode.A }, 300);
            api.Wait(2000);

        }


        [Test, Order(9)]
        public void DestroyingTheThirdColumn()
        {

            api.Wait(1000);
            Vector3 thirddestructibleColumn = api.GetObjectFieldValue<Vector3>("//*[@name='Stones_3']/fn:component('UnityEngine.Transform')/@localPosition");
            Console.WriteLine(thirddestructibleColumn);
            thirddestructibleColumn.x += 1;
            Console.WriteLine(thirddestructibleColumn);
            api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('UnityEngine.Transform')", "position", thirddestructibleColumn);
            api.KeyPress(new KeyCode[] { KeyCode.K }, 100);
            api.Wait(100);
        }


        [Test, Order(10)]
        public void ReachingZone4()
        {
            api.Wait(100);
            api.KeyPress(new KeyCode[] { KeyCode.A }, 450);
            api.Wait(2000);

        }


        [Test, Order(11)]
        public void CollectingTheThirdKey()
        {
            api.Wait(800);
            Vector3 keycollect = api.GetObjectPosition("(//*[@name='Key'])");
            keycollect.y -= 3;

            Console.WriteLine(keycollect.ToString());
            api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('UnityEngine.Transform')", "position", keycollect);
            api.Wait(3000);

            Color expectedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            Color actualColor = api.GetObjectFieldValue<Color>("/*[@name='KeyCanvas']/*[@name='KeyIcon(Clone)'][2]/*[@name='Key']/fn:component('UnityEngine.UI.Image')/@color");

            Assert.That(expectedColor.Equals(actualColor));
        }


        [Test, Order(12)]
        public void ReturningToZone2Again()

        {

            api.Wait(800);
            Vector3 zone4exit = api.GetObjectPosition("(//*[@name='DoorSprite'][1])");
            zone4exit.x += 3;

            Console.WriteLine(zone4exit.ToString());
            api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('UnityEngine.Transform')", "position", zone4exit);
            api.KeyPress(new KeyCode[] { KeyCode.D }, 450);
            api.Wait(3000);

        }


        [Test, Order(13)]
        public void ReachingTheBoss()

        {
            api.Wait(800);
            Vector3 hubdoor = api.GetObjectPosition("(//*[@name='HubDoor'])");

            Console.WriteLine(hubdoor.ToString());
            api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('UnityEngine.Transform')", "position", hubdoor);
            api.KeyPress(new KeyCode[] { KeyCode.E }, 100);
            api.Wait(100);

        }


        [Test, Order(14)]
        public void BossFight()

        {
            api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('Gamekit2D.Damager')", "damage", 30000);

            bool isGolem;
            Vector3 golemPosition;
            api.KeyPress(new KeyCode[] { KeyCode.D }, 10000);
            api.Wait(20000);

            api.WaitForObject("//*[@name='Gunner']");
            api.Wait(2000);

            do
            {
                golemPosition = api.GetObjectPosition("//*[@name='Gunner']");
                //teleporting Ellen on top of the Gunner
                api.SetObjectFieldValue("//*[@name='Ellen']/fn:component('UnityEngine.Transform')", "position", new Vector3(golemPosition.x - 2, golemPosition.y + 1, golemPosition.z));

                for (int i = 0; i < 3; i++)
                {
                    api.KeyPress(new KeyCode[] { KeyCode.K }, 100);
                    api.Wait(100);
                }
                //Healing Ellen to be able to get through the boss fight.
                api.CallMethod(" //*[@name='Ellen']/fn:component('Gamekit2D.Damageable')", "SetHealth", new object[] { 5 });

                try
                {
                    //Trying to acquire value when the boss is beaten throws an exception 
                    isGolem = api.GetObjectFieldValue<bool>("//*[@name='Gunner']/@active");
                }
                catch
                {
                    isGolem = false;
                }

            } while (isGolem);

            
        }


        [OneTimeTearDown]
        public void Disconnect()
        {
            // Disconnect the GameDriver client from the agent
            api.DisableHooks(HookingObject.ALL);
            api.Wait(2000);
            api.Disconnect();
            api.Wait(2000);
        }
    }

}
