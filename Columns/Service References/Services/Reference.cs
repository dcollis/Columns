﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Columns.Services {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:scoreswsdl", ConfigurationName="Services.scoreswsdlPortType")]
    public interface scoreswsdlPortType {
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:scoreswsdl#register", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        string register(string Scores);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:scoreswsdl#getscores", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [return: System.ServiceModel.MessageParameterAttribute(Name="return")]
        string getscores(string gametype);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface scoreswsdlPortTypeChannel : Columns.Services.scoreswsdlPortType, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class scoreswsdlPortTypeClient : System.ServiceModel.ClientBase<Columns.Services.scoreswsdlPortType>, Columns.Services.scoreswsdlPortType {
        
        public scoreswsdlPortTypeClient() {
        }
        
        public scoreswsdlPortTypeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public scoreswsdlPortTypeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public scoreswsdlPortTypeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public scoreswsdlPortTypeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string register(string Scores) {
            return base.Channel.register(Scores);
        }
        
        public string getscores(string gametype) {
            return base.Channel.getscores(gametype);
        }
    }
}
