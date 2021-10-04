import discord
import json
import random

client = discord.Client(max_messages = 100000)
CONFIG = json.loads(open("config.json","r").read())

@client.event
async def on_ready():
    print("I logged in btw")

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
                return
    # delete any potato no no square sent by jacob
    await checkNewPotato(message)

    # triggers and responses
    for entry in CONFIG["responses"]:
        respond = False
        # check triggers
        for trigger in entry["triggers"]:
            if trigger["type"] == "TEXT":
                if message.content == trigger["data"]:
                    respond = True
            
            elif trigger["type"] == "EMBED":
                for embed in message.embeds:
                    if embed.url == trigger["data"]:
                        respond = True

            elif trigger["type"] == "CONTAINS":
                if message.content.__contains__(trigger["data"]):
                    respond = True
        
        if respond == True:
            # pick a random response
            r = random.randint(0,len(entry["responses"])-1)
            m = entry["responses"][r]
            await message.channel.send(m)

@client.event
async def on_message_edit(before, after):
    await checkNewPotato(after)

async def checkNewPotato(message):
    if isinstance(message.author, discord.member.Member): # check if the message is in a server
        if message.author.id == 540915558439190548:
            if (":nono1:" in message.content or ":nono2:" in message.content) and ":newpotato:" in message.content:
                await message.delete()

client.run(CONFIG["discord-token"])