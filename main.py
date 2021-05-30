import asyncio
import threading
from time import sleep
import discord
import json
import random

client = discord.Client(max_messages = 100000)
CONFIG = json.loads(open("config.json","r").read())
loop = asyncio.get_event_loop()

message_sent = 0

@client.event
async def on_ready():
    print("I logged in btw")
    
    if CONFIG["enable-anger"] == False:
        await client.change_presence(status=discord.Status.do_not_disturb,activity=None)
    else:
        await client.change_presence(status=discord.Status.online,activity=None)

@client.event
async def on_message(message):
    global client
    # the actual point of this bot
    for channel in CONFIG["ideas-channels"]:
        if str(message.channel.id) == channel:
            if message.reference == None:
                # now actually do the thing with the reactions
                await message.add_reaction('❌')
                await message.add_reaction('✅')
    #haha funny meme
    if CONFIG["enable-anger"] == True:
        if message.mention_everyone == False:
            for user in message.mentions:
                if type(user) == discord.Member:
                    if user.id == client.user.id:
                        r = random.randint(0,len(CONFIG["angry-messages"])-1)
                        m = CONFIG["angry-messages"][r]

                        global message_sent
                        message_sent += 1
                        
                        if message_sent >= CONFIG["max_messages"]:
                            await client.change_presence(status=discord.Status.do_not_disturb,activity=None)
                            CONFIG["enable-anger"] = False
                            threading.Thread(target=wait).start()
                        
                        await message.channel.send(m)
                        break

def wait():
    global client
    global loop
    global message_sent
    sleep(CONFIG["sleep_time"])
    loop.create_task(client.change_presence(status=discord.Status.online,activity=None))
    message_sent = 0
    CONFIG["enable-anger"] = True

client.run(CONFIG["discord-token"])