const LOCAL_BASE_URL = 'http://localhost:7071';
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
                //const apiUrl = `${getAPIBaseUrl()}/api/getStocks`;
                //const response = await axios.get(apiUrl);
                //console.log('Files fetched from ', apiUrl);
                //app.cognitivepipeline = response.data;
                app.cognitiveFiles = [{
                    "ownerId": "NA",
                    "fileName": "NA",
                    "mediaType": "NA",
                    "isProcessed": "false",
                    "status": "No active processing",
                    "cognitivePipelineActions": [
                        {
                            "serviceType": "OCR"
                        },
                        {
                            "serviceType": "FaceDetection"
                        }
                    ],
                    "origin": "RTC-Client"
                }];
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

    connection.on('cognitivepipelinertc', updatedFile => {
        const index = app.cognitiveFiles.findIndex(s => s.id === updatedFile.id);
        app.cognitiveFiles.splice(index, 1, updatedFile);
    });

    connection.start().then(() => {
        console.log("SignalR connection established");
    });
};

connect();
