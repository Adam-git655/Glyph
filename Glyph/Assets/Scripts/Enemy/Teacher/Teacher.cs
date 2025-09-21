using UnityEngine;

public class Teacher : EnemyAI
{
   protected override void PlayerGetCaught()
   {
      base.PlayerGetCaught();
      print("player get caught");
      
      ScreenTransition.Instance.GoToOldCheckPoint();
   }
}
