﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VkNet.Model;
using VkNet.Utils;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;


   

namespace VkBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        /// <summary>
        /// Конфигурация приложения
        /// </summary>
        private readonly IConfiguration _configuration;
        private readonly IVkApi _vkApi;

     
        public CallbackController(IVkApi vkApi, IConfiguration configuration)
        {
            _vkApi = vkApi;
            _configuration = configuration;
        }
        [HttpPost]
        public IActionResult Callback([FromBody] Updates updates)
        {
            // Тип события
            switch (updates.Type)
            {
                // Ключ-подтверждение
                case "confirmation":
                {
                    return Ok(_configuration["Config:Confirmation"]);
                }

                    // Новое сообщение
                   
                case "message_new":
                    {
                        // Десериализация
                        var msg = Message.FromJson(new VkResponse(updates.Object));
                        HandleAsync(msg);
                      
                        break;
                    }
            }

            return Ok("ok");
        }
        async public void HandleAsync(Message msg)//обработка полученного сообщения
        {

            // Отправим в ответ полученный от пользователя текст
            
            await _vkApi.Messages.SendAsync(new MessagesSendParams
            {
                RandomId = new DateTime().Millisecond,
                PeerId = msg.PeerId.Value,
                Message = "Your text: "+ msg.Text
            });

        }
            public void Handle(Message msg)
        {
          
            // Отправим в ответ полученный от пользователя текст
            _vkApi.Messages.Send(new MessagesSendParams
            {
                RandomId = new DateTime().Millisecond,
                PeerId = msg.PeerId.Value,
                Message = "Your text: " + msg.Text
            });
        }
    }
}
