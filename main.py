import threading
from time import sleep
import discord
import json
import random

client = discord.Client(max_messages = 100000)
CONFIG = json.loads(open("config.json","r").read())

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
        if str(message.channel.id) == CONFIG["angry-channel"]:
            if message.mention_everyone == False:
                for user in message.mentions:
                    if type(user) == discord.Member:
                        if user.id == client.user.id:
                            r = random.randint(0,len(CONFIG["angry-messages"])-1)
                            m = CONFIG["angry-messages"][r]
                            await message.channel.send(m)
                            break

client.run(CONFIG["discord-token"])