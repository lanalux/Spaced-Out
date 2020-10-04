using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour
{
   AudioSource song;
    bool radioOn=true;

   void Start(){
       song = this.GetComponent<AudioSource>();

   }

   public void RadioButton(){
       if(radioOn){
           song.Stop();
           radioOn=false;
       } else {
           song.Play();
           radioOn=true;
       }
   }

}
