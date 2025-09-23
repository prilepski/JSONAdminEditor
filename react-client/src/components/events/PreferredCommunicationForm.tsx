import React from 'react';
import { useQuery } from '@tanstack/react-query';
import axios from 'axios';

interface PreferredCommunicationFormProps {
  preferredCommunication: Array<{ channel: string; priority: number }>;
  onUpdate: (communication: Array<{ channel: string; priority: number }>) => void;
}

export const PreferredCommunicationForm: React.FC<PreferredCommunicationFormProps> = ({
  preferredCommunication,
  onUpdate,
}) => {
  const { data: eventChannels = [] } = useQuery({
    queryKey: ['eventChannels'],
    queryFn: async () => {
      const response = await axios.get('/api/dictionaries/event-channels');
      return response.data;
    }
  });
  
  const channelOptions = eventChannels.map((ch: any) => ch.channelName).filter(Boolean);

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h5>Preferred Communication Channels</h5>
        <button
          className="btn btn-success btn-sm"
          onClick={() => {
            onUpdate([...preferredCommunication, { channel: '', priority: 1 }]);
          }}
        >
          <i className="fas fa-plus me-1"></i>Add Channel
        </button>
      </div>
      <div className="table-responsive">
        <table className="table table-bordered">
          <thead className="table-light">
            <tr>
              <th>Channel</th>
              <th>Priority</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {preferredCommunication.map((comm: any, index) => (
              <tr key={index}>
                <td>
                  <select
                    className="form-select"
                    value={comm.channel || ''}
                    onChange={(e) => {
                      const updated = [...preferredCommunication];
                      updated[index] = { ...comm, channel: e.target.value };
                      onUpdate(updated);
                    }}
                  >
                    <option value="">Select Channel</option>
                    {channelOptions.map(channel => (
                      <option key={channel} value={channel}>{channel}</option>
                    ))}
                  </select>
                </td>
                <td>
                  <input
                    type="number"
                    className="form-control"
                    value={comm.priority || ''}
                    onChange={(e) => {
                      const updated = [...preferredCommunication];
                      updated[index] = { ...comm, priority: parseInt(e.target.value) };
                      onUpdate(updated);
                    }}
                  />
                </td>
                <td>
                  <button
                    className="btn btn-danger btn-sm"
                    onClick={() => {
                      const updated = preferredCommunication.filter((_, i) => i !== index);
                      onUpdate(updated);
                    }}
                  >
                    <i className="fas fa-trash"></i>
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};