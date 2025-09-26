import React, { useState } from 'react';
import { PageHeader, SaveButton, LoadingSpinner } from '../components/common';
import { PageErrorBoundary, ComponentErrorBoundary } from '../components/common';
import { useAgentsQuery, useAgentsMutation } from '../hooks/useAgentsQuery';
import { useErrorHandler } from '../hooks/useErrorHandler';

export const Agents: React.FC = () => {
  const [agents, setAgents] = useState<Record<string, boolean>>({});
  const [isAddingNew, setIsAddingNew] = useState(false);
  const [newAgentName, setNewAgentName] = useState('');

  const { data = {}, isLoading } = useAgentsQuery();
  const mutation = useAgentsMutation();
  const { handleError } = useErrorHandler({ context: 'Agents' });

  React.useEffect(() => {
    setAgents(data);
  }, [data]);

  const handleToggle = (agentName: string) => {
    setAgents(prev => ({
      ...prev,
      [agentName]: !prev[agentName]
    }));
  };

  const handleAddAgent = () => {
    if (newAgentName.trim() && !agents[newAgentName.trim()]) {
      setAgents(prev => ({
        ...prev,
        [newAgentName.trim()]: false
      }));
      setNewAgentName('');
    }
  };

  const handleRemoveAgent = (agentName: string) => {
    setAgents(prev => {
      const { [agentName]: _, ...rest } = prev;
      return rest;
    });
  };

  const handleSave = async () => {
    try {
      await mutation.mutateAsync(agents);
    } catch (error) {
      handleError(error, 'Failed to save agents settings');
    }
  };

  return (
    <PageErrorBoundary pageName="Agents">
      <PageHeader
        icon="fa-robot"
        title="Agents Management"
        description="Manage system agents and their enabled/disabled status for automated processes."
      />

      {isLoading ? (
        <LoadingSpinner text="Loading agents settings..." />
      ) : (
        <div className="card">
          <div className="card-header">
            <h3>
              <i className="fas fa-edit me-2"></i>Agent Settings
            </h3>
          </div>
          <div className="card-body">
            <ComponentErrorBoundary componentName="Agents Editor">
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h5>
                  <i className="fas fa-robot me-2"></i>System Agents
                </h5>
                <div className="d-flex gap-2">
                  <button
                    type="button"
                    className="btn btn-success"
                    onClick={() => setIsAddingNew(true)}
                    disabled={isAddingNew}
                  >
                    <i className="fas fa-plus me-1"></i>Add Agent
                  </button>
                  <SaveButton
                    onClick={handleSave}
                    loading={mutation.isPending}
                    text="Save Changes"
                  />
                </div>
              </div>

              {Object.keys(agents).length === 0 && !isAddingNew ? (
                <div className="text-center py-4">
                  <i className="fas fa-robot fa-3x text-muted mb-3"></i>
                  <h5 className="text-muted">No Agents</h5>
                  <p className="text-muted">
                    No agents found. Click "Add Agent" to create one.
                  </p>
                </div>
              ) : (
                <div className="table-responsive">
                  <table className="table table-striped table-hover">
                    <thead className="table-dark">
                      <tr>
                        <th style={{ width: '70%' }}>Agent Name</th>
                        <th style={{ width: '15%' }}>Status</th>
                        <th style={{ width: '15%' }}>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {Object.entries(agents).map(([agentName, isEnabled]) => (
                        <tr key={agentName}>
                          <td>
                            <input
                              type="text"
                              className="form-control form-control-sm"
                              value={agentName}
                              readOnly
                            />
                          </td>
                          <td className="text-center">
                            <div className="form-check">
                              <input
                                className="form-check-input"
                                type="checkbox"
                                checked={isEnabled}
                                onChange={() => handleToggle(agentName)}
                              />
                            </div>
                          </td>
                          <td className="text-center">
                            <button
                              type="button"
                              className="btn btn-danger btn-sm"
                              onClick={() => handleRemoveAgent(agentName)}
                            >
                              <i className="fas fa-trash"></i>
                            </button>
                          </td>
                        </tr>
                      ))}
                      {isAddingNew && (
                        <tr>
                          <td>
                            <input
                              type="text"
                              className="form-control form-control-sm"
                              value={newAgentName}
                              onChange={(e) => setNewAgentName(e.target.value)}
                              onKeyPress={(e) => {
                                if (e.key === 'Enter') {
                                  if (newAgentName.trim() && !agents[newAgentName.trim()]) {
                                    setAgents(prev => ({ ...prev, [newAgentName.trim()]: false }));
                                    setNewAgentName('');
                                    setIsAddingNew(false);
                                  }
                                }
                              }}
                              onBlur={() => {
                                if (newAgentName.trim() && !agents[newAgentName.trim()]) {
                                  setAgents(prev => ({ ...prev, [newAgentName.trim()]: false }));
                                }
                                setNewAgentName('');
                                setIsAddingNew(false);
                              }}
                              placeholder="Enter agent name"
                              autoFocus
                            />
                          </td>
                          <td className="text-center">
                            <div className="form-check">
                              <input
                                className="form-check-input"
                                type="checkbox"
                                checked={false}
                                disabled
                              />
                            </div>
                          </td>
                          <td className="text-center">
                            <button
                              type="button"
                              className="btn btn-secondary btn-sm"
                              onClick={() => {
                                setNewAgentName('');
                                setIsAddingNew(false);
                              }}
                            >
                              <i className="fas fa-times"></i>
                            </button>
                          </td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                </div>
              )}
            </ComponentErrorBoundary>
          </div>
        </div>
      )}
    </PageErrorBoundary>
  );
};