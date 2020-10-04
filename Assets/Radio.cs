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
       } else {
           song.Play();
       }
   }

}
