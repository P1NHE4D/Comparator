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
                sh "dotnet test Comparator/ComparatorTest.csproj --logger \"trx;LogFileName=unit_tests.trx\""
                step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: false, keepLongStdio: true])
            }
        }
    }
}
