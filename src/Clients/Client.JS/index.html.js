const LOCAL_BASE_URL = 'http://localhost:5860';
const AZURE_BASE_URL = '<FUNCTION_APP_ENDPOINT>';

const getAPIBaseUrl = () => {
    const isLocal = /localhost/.test(window.location.href);
    return isLocal ? LOCAL_BASE_URL : AZURE_BASE_URL;
}

const app = new Vue({
    el: '#app',
    data() { 
        return {
            cognitiveFiles: []
        }
    },
    methods: {
        async getFiles() {
            try {
                this.cognitiveFiles = [];

                //Demo data if you want to see the look
                // this.cognitiveFiles = [{
                //             "id": "1",
                //             "ownerId": "NA",
                //             "fileName": "SAMPLE1.PNG",
                //             "mediaType": "Image",
                //             "isProcessed": false,
                //             "status": "Submitted",
                //             "cognitivePipelineActions": [
                //                 {
                //                     "serviceType": "OCR",
                //                     "isSuccessful": true
                //                 },
                //                 {
                //                     "serviceType": "FaceDetection",
                //                     "isSuccessful": true
                //                 }
                //             ],
                //             "origin": "RTC-Client"
                //         },
                //         {
                //             "id": "2",
                //             "ownerId": "NA",
                //             "fileName": "SAMPLE2.MP4",
                //             "mediaType": "Video",
                //             "isProcessed": true,
                //             "status": "Processed",
                //             "cognitivePipelineActions": [
                //                 {
                //                     "serviceType": "OCR",
                //                     "isSuccessful": false
                //                 },
                //                 {
                //                     "serviceType": "FaceDetection",
                //                     "isSuccessful": true
                //                 },
                //                 {
                //                     "serviceType": "FaceVerification",
                //                     "isSuccessful": true
                //                 },
                //                 {
                //                     "serviceType": "ObjectDetection",
                //                     "isSuccessful": true
                //                 },
                //                 {
                //                     "serviceType": "CustomVisionClassification",
                //                     "isSuccessful": true
                //                 },
                //                 {
                //                     "serviceType": "CustomVisionObjectDetection",
                //                     "isSuccessful": true
                //                 },
                //             ],
                //             "origin": "RTC-Client"
                //         }];
            } catch (ex) {
                console.error(ex);
            }
        }
    },
    created() {
        this.getFiles();
    }
});

const connect = () => {
    const connection = new signalR.HubConnectionBuilder().withUrl(`${getAPIBaseUrl()}/api`).build();

    connection.onclose(()  => {
        console.log('SignalR connection disconnected');
        setTimeout(() => connect(), 2000);
    });

    connection.on('UpdateNotification', updatedFile => {
        console.log("Received UpdateNotification");
        const index = app.cognitiveFiles.findIndex(s => s.id === updatedFile.id);
        if(index >= 0)
        {app.cognitiveFiles.splice(index, 1, updatedFile);}
        else
        {app.cognitiveFiles.push(updatedFile);}
    });

    connection.start().then(() => {
        console.log("SignalR connection established");
    });
};

connect();
