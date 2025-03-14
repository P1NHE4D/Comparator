pipeline {
    agent any
    environment {
    dotnet = '/usr/bin/dotnet'
    }
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        stage('Restore Packages') {
            steps {
                sh "dotnet restore Comparator/Comparator.csproj"
            }
        }
        stage('Clean') {
            steps {
                sh "dotnet clean Comparator/Comparator.csproj"
            }
        }
        stage('Build') {
            steps {
                sh "dotnet build Comparator/Comparator.csproj --configuration Release"
            }
        }
        stage('Test') {
            steps {
                sh "dotnet test ComparatorTest/ComparatorTest.csproj --logger \"trx;LogFileName=unit_tests.trx\""
                step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: false, keepLongStdio: true])
            }
        }
        
        stage('Deploy') {
            when { branch 'master' }
            steps {
                sh "sudo systemctl stop comparator.service"
                sh "dotnet publish --configuration Release --output /srv/comparator"
                sh "sudo systemctl restart comparator.service"
            }
        }
    }
}
