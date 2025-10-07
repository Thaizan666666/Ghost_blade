using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Ghost_blade
{
    public static class Sound
    {
        // Spund Player
        public static SoundEffect ULT { get; private set; }
        public static SoundEffect p_shooting { get; private set; }
        public static SoundEffect sounddash { get; private set; }

        // Sound Enemy
        public static SoundEffect soundhitenemy { get; private set; }
        public static SoundEffect s_shooting { get; private set; }
        public static SoundEffect s_enemydie { get; private set; }
        public static SoundEffect player_hit { get; private set; }
        public static SoundEffect gun_reload { get; private set; }
        public static SoundEffect parry { get; private set; }
        public static SoundEffect parry_got { get; private set; }
        public static SoundEffect attack { get; private set; }
        public static SoundEffect Change_weapon { get; private set; }
        public static SoundEffect enemy_attack_punch { get; private set; }
        public static SoundEffect enemy_attack_laser { get; private set; }
        public static SoundEffect ingame { get; private set; }
        public static SoundEffectInstance ingameMusicInstance;
        public static SoundEffect gameover { get; private set; }
        public static SoundEffectInstance gameoverMusicInstance;
        public static SoundEffect lobby { get; private set; }
        public static SoundEffectInstance lobbyMusicInstance;
        public static SoundEffect boss { get; private set; }
        public static SoundEffectInstance _bossMusicInstance;
        public static SoundEffectInstance _bossExplosionInstance;
        public static SoundEffect boss_explosio { get; private set; }
        public static SoundEffect Gatling_gun { get; private set; }
        public static SoundEffectInstance Gatling_gunMusicInstance;
        public static SoundEffect laser_boss { get; private set; }
        public static SoundEffectInstance laser_bossMusicInstance;

        public static void LoadContent(ContentManager content)
        {
            // โหลด SoundEffect ตามชื่อไฟล์ใน Content Pipeline
            // ตรวจสอบว่าใช้ 'content' (ตัวแปรที่รับเข้ามา) ในการโหลด
            ULT = content.Load<SoundEffect>("Ultimate");
            p_shooting = content.Load<SoundEffect>("player_attack_gun");
            sounddash = content.Load<SoundEffect>("dash");
            soundhitenemy = content.Load<SoundEffect>("enemy_hit");
            s_shooting = content.Load<SoundEffect>("enemy_attack_gun");
            s_enemydie = content.Load<SoundEffect>("enemy_explosion");
            player_hit = content.Load<SoundEffect>("player_hit");
            gun_reload = content.Load<SoundEffect>("gun_reload");
            parry = content.Load<SoundEffect>("parry");
            parry_got = content.Load<SoundEffect>("parry_got");
            attack = content.Load<SoundEffect>("attack");
            Change_weapon = content.Load<SoundEffect>("Change_weapon");
            enemy_attack_punch = content.Load<SoundEffect>("enemy_attack_punch");
            enemy_attack_laser = content.Load<SoundEffect>("enemy_attack_laser");
            ingame = content.Load<SoundEffect>("ingame");
            gameover = content.Load<SoundEffect>("gameover");
            lobby = content.Load<SoundEffect>("lobby");
            boss = content.Load<SoundEffect>("boss");
            boss_explosio = content.Load<SoundEffect>("boss_explosio");
            Gatling_gun = content.Load<SoundEffect>("Gatling_gun ");
            laser_boss = content.Load<SoundEffect>("laser_boss");
        }
        public static void Play(SoundEffect effect, float volume = 1.0f)
        {
            if (effect != null)
            {
                // สามารถเพิ่มเงื่อนไขอื่นๆ เช่น ปริมาณสูงสุดของเสียงที่เล่นพร้อมกัน (Max Sound Instances)
                effect.Play(volume, 0.0f, 0.0f);
            }
        }
        public static SoundEffectInstance Played(SoundEffect effect, float volume = 1.0f)
        {
            if (effect == null)
            {
                return null;
            }

            // 1. สร้าง Instance จาก SoundEffect
            SoundEffectInstance instance = effect.CreateInstance();

            // 2. ตั้งค่าการวนซ้ำ (IsLooped = false)
            instance.IsLooped = false;

            // 3. ตั้งค่า Volume (และ Pitch/Pan)
            instance.Volume = volume;
            instance.Pitch = 0.0f;
            instance.Pan = 0.0f;

            // 4. สั่งเล่น
            instance.Play();

            return instance;
        }
        public static SoundEffectInstance Loop(SoundEffect effect, float volume = 1.0f)
        {
            if (effect == null)
            {
                return null;
            }

            // 1. สร้าง Instance จาก SoundEffect
            SoundEffectInstance instance = effect.CreateInstance();

            // 2. ตั้งค่าการวนซ้ำ (IsLooped = true)
            instance.IsLooped = true;

            // 3. ตั้งค่า Volume (และ Pitch/Pan)
            instance.Volume = volume;
            instance.Pitch = 0.0f;
            instance.Pan = 0.0f;

            // 4. สั่งเล่น
            instance.Play();

            // 5. คืนค่า Instance กลับไปให้คลาสที่เรียกใช้ เพื่อใช้ในการสั่ง Stop ภายหลัง
            return instance;
        }
        public static void StopLoop(SoundEffectInstance instance)
        {
            if (instance != null && instance.State == SoundState.Playing)
            {
                instance.Stop();
            }
        }
        public static void PauseLoop(SoundEffectInstance instance)
        {
            // ตรวจสอบว่า Instance มีอยู่ และกำลังเล่นอยู่ (Playing)
            if (instance != null && instance.State == SoundState.Playing)
            {
                instance.Pause(); // สั่งหยุดชั่วคราว
            }
        }
        public static void ResumeLoop(SoundEffectInstance instance)
        {
            // ตรวจสอบว่า Instance มีอยู่ และถูกหยุดชั่วคราวไว้ (Paused)
            if (instance != null && instance.State == SoundState.Paused)
            {
                instance.Resume(); // สั่งเล่นต่อ
            }
        }
        public static void PauseAllLoopingSounds()
        {
            PauseLoop(_bossMusicInstance);
            PauseLoop(_bossExplosionInstance);
            PauseLoop(Gatling_gunMusicInstance);
            PauseLoop(laser_bossMusicInstance);
        }

        public static void ResumeAllLoopingSounds()
        {
            ResumeLoop(_bossMusicInstance);
            ResumeLoop(_bossExplosionInstance);
            ResumeLoop(Gatling_gunMusicInstance);
            ResumeLoop(laser_bossMusicInstance);
        }
        public static void StopAllLoopingSounds()
        {
            StopLoop(_bossMusicInstance);
            StopLoop(_bossExplosionInstance);
            StopLoop(Gatling_gunMusicInstance);
            StopLoop(laser_bossMusicInstance);
        }
    }
}