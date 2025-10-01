import React, { useState } from 'react';
import { LoadingSpinner, SaveButton, ComponentErrorBoundary } from './index';
import { ContentVariablesTable } from './ContentVariablesTable';
import { NestedVariablesNavigator } from './NestedVariablesNavigator';
import { AddCategoryModal } from './AddCategoryModal';

interface NestedVariablesEditorProps {
  data: Record<string, Record<string, Record<string, string>>>;
  onSave: (category: string, subcategory: string, variables: Record<string, string>) => Promise<void>;
  onAddCategory: (name: string) => Promise<void>;
  onAddSubcategory: (category: string, name: string) => Promise<void>;
  onDeleteSubcategory?: (category: string, subcategory: string) => Promise<void>;
  isLoading?: boolean;
  isSaving?: boolean;
}

export const NestedVariablesEditor: React.FC<NestedVariablesEditorProps> = ({
  data,
  onSave,
  onAddCategory,
  onAddSubcategory,
  onDeleteSubcategory,
  isLoading = false,
  isSaving = false,
}) => {
  const [selectedPath, setSelectedPath] = useState<string>('');
  const [expandedCategories, setExpandedCategories] = useState<Set<string>>(new Set());
  const [newCategoryName, setNewCategoryName] = useState('');
  const [newSubcategoryName, setNewSubcategoryName] = useState('');
  const [showAddCategory, setShowAddCategory] = useState(false);
  const [showAddSubcategory, setShowAddSubcategory] = useState('');

  const [category, subcategory] = selectedPath.split('/');
  const currentVariables = category && subcategory ? data?.[category]?.[subcategory] || {} : {};

  const toggleCategory = (cat: string) => {
    const newExpanded = new Set(expandedCategories);
    if (newExpanded.has(cat)) {
      newExpanded.delete(cat);
    } else {
      newExpanded.add(cat);
    }
    setExpandedCategories(newExpanded);
  };

  const handleAddCategory = async () => {
    await onAddCategory(newCategoryName);
    setNewCategoryName('');
    setShowAddCategory(false);
  };

  const handleAddSubcategory = async () => {
    await onAddSubcategory(showAddSubcategory, newSubcategoryName);
    setNewSubcategoryName('');
    setShowAddSubcategory('');
  };

  const handleDeleteSubcategory = async (category: string, subcategory: string) => {
    if (onDeleteSubcategory) {
      await onDeleteSubcategory(category, subcategory);
      if (selectedPath === `${category}/${subcategory}`) {
        setSelectedPath('');
      }
    }
  };

  const [currentVariablesState, setCurrentVariablesState] = useState<Record<string, string>>({});

  // Update local state when currentVariables changes
  React.useEffect(() => {
    setCurrentVariablesState(currentVariables);
  }, [JSON.stringify(currentVariables)]);

  const handleUpdate = (variables: Record<string, string>) => {
    setCurrentVariablesState(variables);
  };

  const handleSave = async () => {
    if (category && subcategory) {
      await onSave(category, subcategory, currentVariablesState);
    }
  };

  if (isLoading) {
    return <LoadingSpinner text="Loading data..." />;
  }

  if (Object.keys(data).length === 0) {
    return (
      <div className="alert alert-info">
        No content variables overrides found.
        <button className="btn btn-primary btn-sm ms-2" onClick={() => setShowAddCategory(true)}>
          <i className="fas fa-plus me-1"></i>Add First Category
        </button>
        <AddCategoryModal
          show={showAddCategory}
          title="Add New Category"
          placeholder="Category name"
          value={newCategoryName}
          onValueChange={setNewCategoryName}
          onConfirm={handleAddCategory}
          onCancel={() => { setShowAddCategory(false); setNewCategoryName(''); }}
          isLoading={isSaving}
        />
      </div>
    );
  }

  return (
    <ComponentErrorBoundary componentName="Nested Variables Editor">
      <div className="row">
        <div className="col-md-4">
          <NestedVariablesNavigator
            data={data}
            selectedPath={selectedPath}
            expandedCategories={expandedCategories}
            onPathSelect={setSelectedPath}
            onToggleCategory={toggleCategory}
            onAddCategory={() => setShowAddCategory(true)}
            onAddSubcategory={setShowAddSubcategory}
            onDeleteSubcategory={handleDeleteSubcategory}
          />
        </div>

        <div className="col-md-8">
          {selectedPath ? (
            <ContentVariablesTable
              contentVariables={currentVariablesState}
              onUpdate={handleUpdate}
              title={`${category} > ${subcategory}`}
              variableColumnHeader={subcategory}
              showIsRedefined={false}
              saveButton={
                <SaveButton
                  onClick={handleSave}
                  loading={isSaving}
                  text="Save Changes"
                />
              }
            />
          ) : (
            <div className="card">
              <div className="card-body text-center py-5">
                <i className="fas fa-mouse-pointer fa-3x text-muted mb-3"></i>
                <h5 className="text-muted">Select a Category</h5>
                <p className="text-muted">Choose a category and subcategory from the sidebar to edit variables.</p>
              </div>
            </div>
          )}
        </div>
      </div>

      <AddCategoryModal
        show={showAddCategory}
        title="Add New Category"
        placeholder="Category name"
        value={newCategoryName}
        onValueChange={setNewCategoryName}
        onConfirm={handleAddCategory}
        onCancel={() => { setShowAddCategory(false); setNewCategoryName(''); }}
        isLoading={isSaving}
      />

      <AddCategoryModal
        show={!!showAddSubcategory}
        title={`Add Subcategory to ${showAddSubcategory}`}
        placeholder="Subcategory name"
        value={newSubcategoryName}
        onValueChange={setNewSubcategoryName}
        onConfirm={handleAddSubcategory}
        onCancel={() => { setShowAddSubcategory(''); setNewSubcategoryName(''); }}
        isLoading={isSaving}
      />
    </ComponentErrorBoundary>
  );
};