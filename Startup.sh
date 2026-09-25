#!/bin/bash
ROOTDIRECTORY=$(cd -P -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd -P)
cd "$ROOTDIRECTORY"/NalamVazha.Models
echo "The current directory is : Expecting Model folder"
pwd
echo cma@2020# | sudo -S  dotnet build
cd "$ROOTDIRECTORY"/NalamVazha.DAL
echo "The current directory is : Expecting DAL folder"
pwd
echo cma@2020# | sudo -S  dotnet build
cd "$ROOTDIRECTORY"/
echo cma@2020# | sudo -S  rm -f "$ROOTDIRECTORY"/NalamVazha.sln
echo cma@2020# | sudo -S  dotnet new sln --name NalamVazha
echo cma@2020# | sudo -S  dotnet sln add "$ROOTDIRECTORY"/NalamVazha.Models/NalamVazha.Models.csproj
echo cma@2020# | sudo -S  dotnet sln add "$ROOTDIRECTORY"/NalamVazha.DAL/NalamVazha.DAL.csproj
echo cma@2020# | sudo -S  dotnet sln add "$ROOTDIRECTORY"/Storage.Interface/Storage.Interface.csproj
echo cma@2020# | sudo -S  dotnet sln add "$ROOTDIRECTORY"/Storage.Azure/Storage.Azure.csproj

                    cd "$ROOTDIRECTORY"/NalamVazhaWebApi
                    echo cma@2020# | sudo -S  dotnet build
                    echo cma@2020# | sudo -S  dotnet publish -o "$ROOTDIRECTORY"/Publish/WebApi
cd "$ROOTDIRECTORY"
                cd "$ROOTDIRECTORY"/Admin
                echo cma@2020# | sudo -S  dotnet build
                echo cma@2020# | sudo -S  dotnet publish -o "$ROOTDIRECTORY"/Publish/Admin
                echo cma@2020# | sudo -S  mkdir "$ROOTDIRECTORY"/Publish/Admin/wwwroot/uploads
                echo cma@2020# | sudo -S  cp /home/ubuntu/Automaton/AutomatonClient/wwwroot/BackupFiles/NalamVazha/AdminUploads/*.* "$ROOTDIRECTORY"/Publish/Admin/wwwroot/uploads


echo "Setting up the Publish Evnrionment"
                        cd /home/ubuntu/Automaton/AutomatonClient/wwwroot/PublishedFiles
                        echo cma@2020# | sudo -S  chown -R ubuntu NalamVazha
                        cd "$ROOTDIRECTORY"
                        echo cma@2020# | sudo -S  rm -f /etc/nginx/sites-enabled/thajes
                        echo cma@2020# | sudo -S  rm -f /etc/supervisor/conf.d/NalamVazhaWebApi.conf
                        echo cma@2020# | sudo -S  rm -f /etc/supervisor/conf.d/NalamVazhaClient.conf
                        echo cma@2020# | sudo -S  rm -f /etc/supervisor/conf.d/NalamVazhaAdmin.conf
                        echo cma@2020# | sudo -S  cp "$ROOTDIRECTORY"/PublishRequisites/*.conf /etc/supervisor/conf.d/
                        echo cma@2020# | sudo -S  cp "$ROOTDIRECTORY"/PublishRequisites/thajes /etc/nginx/sites-enabled/
                        echo cma@2020# | sudo -S  supervisorctl reread
                        echo cma@2020# | sudo -S  supervisorctl update
                        echo cma@2020# | sudo -S  supervisorctl restart NalamVazhaWebApi
                        echo cma@2020# | sudo -S  supervisorctl restart NalamVazhaClient
                        echo cma@2020# | sudo -S  supervisorctl restart NalamVazhaAdmin 
                        echo cma@2020# | sudo -S  service nginx reload
curl -v --header "Connection: keep-alive" "https://live.mahat.ai/ContactUs/sentPublishedNotification?projectid=4699b530-c238-4d20-ba5f-cb056b083dab"
sudo -s /home/ubuntu/Automaton/AutomatonClient/wwwroot/git.sh NalamVazha "2026-03-10 11:35" https://thajes:glpat-t87uFGd9A1hDa4Qy6-F7Dm86MQp1OmQH.01.0w1pb18vq@git.mahat.ai/thajes/NalamVazha thajes glpat-t87uFGd9A1hDa4Qy6-F7Dm86MQp1OmQH.01.0w1pb18vq

